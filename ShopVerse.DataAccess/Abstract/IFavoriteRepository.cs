using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Abstract;

public interface IFavoriteRepository:IGenericRepository<Favorite>
{
    List<Favorite> GetFavoritesWithProducts(string userId);
}
