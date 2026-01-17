using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;
using ShopVerse.Entities.Enums;

namespace ShopVerse.Business.Concrete;

public class OrderManager:GenericManager<Order>,IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductService _productService;

    public OrderManager(IOrderRepository orderRepository, IProductService productService) : base(orderRepository)
    {
        _orderRepository = orderRepository;
        _productService = productService;
    }

    public override async Task AddAsync(Order order)
    {
        if (order.OrderDetails != null)
        {
            foreach (var item in order.OrderDetails)
            {
                var product = await _productService.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    if (product.Stock < item.Quantity)
                    {
                        throw new Exception($"'{product.Name}' ürünü için stok yetersiz! (Kalan: {product.Stock})");
                    }

                    product.Stock -= item.Quantity;

                    if (product.Stock < 0) product.Stock = 0;

                    await _productService.UpdateAsync(product);
                }
            }
        }

        await base.AddAsync(order);
    }

    public List<Order> GetOrdersByUserId(string userId)
    {
        return _orderRepository.GetOrdersByUserId(userId);
    }

    public Order GetOrderWithDetails(int orderId)
    {
        return _orderRepository.GetOrderWithDetails(orderId);
    }

    public decimal GetTotalTurnover()
    {
        return _orderRepository.GetTotalTurnover();
    }

    public int GetTotalOrderCount()
    {
        return _orderRepository.GetTotalOrderCount();
    }
    public List<SalesChartDto> GetSalesTrend(int lastMonths = 6)
    {
        return _orderRepository.GetSalesTrend(lastMonths);
    }

    public async Task CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null) throw new Exception("Sipariş bulunamadı.");

       
        if (order.OrderStatus != OrderStatus.Pending)
        {
            throw new Exception("Bu sipariş onaylandığı veya kargoya verildiği için iptal edilemez. Lütfen müşteri hizmetleri ile iletişime geçin.");
        }

        order.OrderStatus = OrderStatus.Canceled;
        await _orderRepository.UpdateAsync(order);
    }


    public async Task ReturnOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Sipariş bulunamadı.");

        if (order.OrderStatus != OrderStatus.Delivered)
        {
            throw new Exception("Sipariş henüz teslim edilmediği için iade edilemez.");
        }

        if (order.DeliveryDate == null)
        {
            
            throw new Exception("Teslimat tarihi sistemde bulunamadı.");
        }

        var daysPassed = (DateTime.Now - order.DeliveryDate.Value).TotalDays;
        if (daysPassed > 3)
        {
            throw new Exception($"İade süresi (3 gün) dolmuştur. ({daysPassed:0} gün geçti)");
        }

        order.OrderStatus = OrderStatus.Refunded; 
        await _orderRepository.UpdateAsync(order);
    }

    
    public async Task AdminUpdateOrderStatus(int orderId, OrderStatus newState)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Sipariş bulunamadı.");

        
        if (order.OrderStatus == OrderStatus.Canceled)
        {
            throw new Exception("Bu sipariş iptal edildiği için durumu değiştirilemez.");
        }

        if (order.OrderStatus == OrderStatus.Refunded)
        {
            throw new Exception("İade işlemi tamamlanmış bir siparişin durumu değiştirilemez.");
        }

        
        if (newState == OrderStatus.Canceled && order.OrderStatus != OrderStatus.Pending)
        {
            throw new Exception("Sadece 'Beklemede' (Pending) olan siparişler iptal edilebilir. Onaylanmış veya kargolanmış siparişler iptal edilemez.");
        }

        if (newState == OrderStatus.Delivered && order.OrderStatus != OrderStatus.Delivered)
        {
            order.DeliveryDate = DateTime.Now;
        }

        order.OrderStatus = newState;
        await _orderRepository.UpdateAsync(order);
    }
}
