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
        // Veritabanından EN GÜNCEL halini çekiyoruz
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null) throw new Exception("Sipariş bulunamadı.");

        // --- KRİTİK KONTROL ---
        // Kullanıcının ekranında "Bekliyor" yazsa bile, 
        // veritabanında Admin bunu "Onaylandı" veya "Kargoda" yaptıysa iptal edemesin.
        if (order.OrderStatus != OrderStatus.Pending)
        {
            throw new Exception("Bu sipariş onaylandığı veya kargoya verildiği için iptal edilemez. Lütfen müşteri hizmetleri ile iletişime geçin.");
        }

        // Kontrolü geçtiyse durumu güncelle
        order.OrderStatus = OrderStatus.Canceled;
        await _orderRepository.UpdateAsync(order);
    }

    // =========================================================
    // 2. KULLANICI İADE İŞLEMİ (3 GÜN KURALI)
    // =========================================================
    public async Task ReturnOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Sipariş bulunamadı.");

        // Sadece "Teslim Edildi" durumundaysa iade olabilir
        if (order.OrderStatus != OrderStatus.Delivered)
        {
            throw new Exception("Sipariş henüz teslim edilmediği için iade edilemez.");
        }

        // Teslim tarihi kontrolü (Yoksa hata vermesin, bugünün tarihini baz alsın veya hata fırlatsın)
        if (order.DeliveryDate == null)
        {
            // Admin teslim tarihini girmemiş olabilir, güvenlik için şu an iadeye izin verelim veya vermeyelim.
            // Biz burada izin vermeyelim, Admin'in düzeltmesini isteyelim.
            throw new Exception("Teslimat tarihi sistemde bulunamadı.");
        }

        // 3 Gün kontrolü
        var daysPassed = (DateTime.Now - order.DeliveryDate.Value).TotalDays;
        if (daysPassed > 3)
        {
            throw new Exception($"İade süresi (3 gün) dolmuştur. ({daysPassed:0} gün geçti)");
        }

        order.OrderStatus = OrderStatus.Refunded; // Veya Returned
        await _orderRepository.UpdateAsync(order);
    }

    // =========================================================
    // 3. ADMIN STATÜ GÜNCELLEME (ÇAKIŞMA KONTROLÜ)
    // =========================================================
    public async Task AdminUpdateOrderStatus(int orderId, OrderStatus newState)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) throw new Exception("Sipariş bulunamadı.");

        // KURAL 1: Kullanıcı iptal ettiyse Admin değiştiremesin
        if (order.OrderStatus == OrderStatus.Canceled)
        {
            throw new Exception("Bu sipariş iptal edildiği için durumu değiştirilemez.");
        }

        // KURAL 2: İade edilmiş bir siparişin durumu değiştirilemez (YENİ KURAL)
        if (order.OrderStatus == OrderStatus.Refunded)
        {
            throw new Exception("İade işlemi tamamlanmış bir siparişin durumu değiştirilemez.");
        }

        // KURAL 3: İptal Etme Kuralı (Sadece 'Bekleyen' ise iptal edilebilir)
        // Eğer Admin yeni durumu "Canceled" yapmaya çalışıyorsa:
        if (newState == OrderStatus.Canceled && order.OrderStatus != OrderStatus.Pending)
        {
            throw new Exception("Sadece 'Beklemede' (Pending) olan siparişler iptal edilebilir. Onaylanmış veya kargolanmış siparişler iptal edilemez.");
        }

        // Teslim tarihi kaydı
        if (newState == OrderStatus.Delivered && order.OrderStatus != OrderStatus.Delivered)
        {
            order.DeliveryDate = DateTime.Now;
        }

        order.OrderStatus = newState;
        await _orderRepository.UpdateAsync(order);
    }
}
