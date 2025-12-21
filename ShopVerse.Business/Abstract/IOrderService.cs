using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Abstract;

public interface IOrderService:IGenericService<Order>
{
    List<Order>GetOrdersByUserId(string userId);
    Order GetOrderWithDetails(int orderId);

}
