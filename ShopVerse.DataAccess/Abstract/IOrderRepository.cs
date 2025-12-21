using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Abstract;

public interface IOrderRepository:IGenericRepository<Order>
{
    List<Order> GetOrdersByUserId(string userId);
    Order GetOrderWithDetails(int id);
}
