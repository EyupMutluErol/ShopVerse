using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.DataAccess.Abstract;

public interface IOrderRepository:IGenericRepository<Order>
{
    List<Order> GetOrdersByUserId(string userId);
    Order GetOrderWithDetails(int id);
    decimal GetTotalTurnover();
    int GetTotalOrderCount();
    List<SalesChartDto> GetSalesTrend(int lastMonths);
}
