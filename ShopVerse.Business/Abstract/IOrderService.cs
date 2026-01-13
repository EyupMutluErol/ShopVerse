using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.Business.Abstract;

public interface IOrderService:IGenericService<Order>
{
    List<Order>GetOrdersByUserId(string userId);
    Order GetOrderWithDetails(int orderId);
    decimal GetTotalTurnover();
    int GetTotalOrderCount();
    List<SalesChartDto> GetSalesTrend(int lastMonths = 6);
}
