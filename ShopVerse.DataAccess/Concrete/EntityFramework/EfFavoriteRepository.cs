using Microsoft.EntityFrameworkCore;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfFavoriteRepository : EfGenericRepository<Favorite>, IFavoriteRepository
{
    private readonly ShopVerseContext _context;
    public EfFavoriteRepository(ShopVerseContext context) : base(context)
    {
        _context = context;
    }

    public List<Favorite> GetFavoritesWithProducts(string userId)
    {
        return _context.Favorites
            .Include(x => x.Product)
            .ThenInclude(p => p.Category)
            .Where(x => x.UserId == userId)
            .ToList();
    }
}
