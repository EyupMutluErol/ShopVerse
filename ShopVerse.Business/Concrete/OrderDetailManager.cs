using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class OrderDetailManager:GenericManager<OrderDetail>,IOrderDetailService
{
    private readonly IOrderDetailRepository _orderDetailRepository;

    public OrderDetailManager(IOrderDetailRepository orderDetailRepository):base(orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
    }
}
