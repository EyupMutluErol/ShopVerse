using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class OrderManager:GenericManager<Order>,IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderManager(IOrderRepository orderRepository):base(orderRepository)
    {
        _orderRepository = orderRepository;
    }
}
