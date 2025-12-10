using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class FavoriteManager:GenericManager<Favorite>,IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteManager(IFavoriteRepository favoriteRepository):base(favoriteRepository)
    {
        _favoriteRepository = favoriteRepository;
    }
}
