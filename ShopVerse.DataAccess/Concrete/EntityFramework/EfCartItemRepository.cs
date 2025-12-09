using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfCartItemRepository : EfGenericRepository<CartItem>, ICartItemRepository
{
    public EfCartItemRepository(ShopVerseContext context) : base(context)
    {
    }
}
