using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfOrderDetailRepository : EfGenericRepository<OrderDetail>, IOrderDetailRepository
{
    public EfOrderDetailRepository(ShopVerseContext context) : base(context)
    {
    }
}
