using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;
using ShopVerse.Entities.Enums;

namespace ShopVerse.Business.Abstract;

public interface IOrderService:IGenericService<Order>
{
    List<Order>GetOrdersByUserId(string userId);
    Order GetOrderWithDetails(int orderId);
    decimal GetTotalTurnover();
    int GetTotalOrderCount();
    List<SalesChartDto> GetSalesTrend(int lastMonths = 6);
    Task CancelOrderAsync(int orderId);

    // 2. Kullanıcının siparişi iade etmesi (Teslim sonrası 3 gün kuralı)
    Task ReturnOrderAsync(int orderId);

    // 3. Admin'in durum güncellemesi (Kritik kontroller içerir)
    Task AdminUpdateOrderStatus(int orderId, OrderStatus newState);
}
