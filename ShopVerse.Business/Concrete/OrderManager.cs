using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

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
        // Listemizin adı OrderDetails olduğu için burayı düzelttik
        if (order.OrderDetails != null)
        {
            foreach (var item in order.OrderDetails)
            {
                // 1. Sipariş edilen ürünü veritabanından buluyoruz
                var product = await _productService.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    // 2. Stok Kontrolü: Yeterli stok var mı?
                    if (product.Stock < item.Quantity)
                    {
                        throw new Exception($"'{product.Name}' ürünü için stok yetersiz! (Kalan: {product.Stock})");
                    }

                    // 3. Stoktan Düşme İşlemi
                    product.Stock -= item.Quantity;

                    // Güvenlik: Stok eksiye düşerse 0'a eşitle
                    if (product.Stock < 0) product.Stock = 0;

                    // 4. Güncellenen ürün stoğunu veritabanına kaydediyoruz
                    await _productService.UpdateAsync(product);
                }
            }
        }

        // 5. Tüm stok işlemleri bitince siparişin kendisini kaydediyoruz
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
}
