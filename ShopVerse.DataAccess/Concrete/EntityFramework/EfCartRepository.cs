using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfCartRepository : EfGenericRepository<Cart>, ICartRepository
{
    public EfCartRepository(ShopVerseContext context) : base(context)
    {
    }
}
