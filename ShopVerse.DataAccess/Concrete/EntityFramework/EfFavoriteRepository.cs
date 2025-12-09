using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfFavoriteRepository : EfGenericRepository<Favorite>, IFavoriteRepository
{
    public EfFavoriteRepository(ShopVerseContext context) : base(context)
    {
    }
}
