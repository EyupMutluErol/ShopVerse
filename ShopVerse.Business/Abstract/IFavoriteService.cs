using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Abstract;

public interface IFavoriteService:IGenericService<Favorite>
{
    Task<List<Favorite>> GetFavoritesWithProductsAsync(string userId);
    Task ToggleFavoriteAsync(string userId, int productId);
    Task<Favorite> GetByProductAndUserAsync(int productId, string userId);
    Task<int> GetCountByUserIdAsync(string userId);
}
