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

    public async Task<List<Favorite>> GetFavoritesWithProductsAsync(string userId)
    {
        return _favoriteRepository.GetFavoritesWithProducts(userId);
    }

    public async Task<Favorite> GetByProductAndUserAsync(int productId, string userId)
    {
        var favorites = await base.GetAllAsync(x => x.ProductId == productId && x.UserId == userId);
        return favorites.FirstOrDefault();
    }

    public async Task<int> GetCountByUserIdAsync(string userId)
    {
        var favorites = await base.GetAllAsync(x => x.UserId == userId);
        return favorites.Count;
    }

    public async Task ToggleFavoriteAsync(string userId, int productId)
    {
        var existingFavorite = await GetByProductAndUserAsync(productId, userId);

        if (existingFavorite != null)
        {
            await base.DeleteAsync(existingFavorite);
        }
        else
        {
            var newFavorite = new Favorite
            {
                UserId = userId,
                ProductId = productId,
                CreatedDate = DateTime.Now,
            };
            await base.AddAsync(newFavorite);
        }
    }
}
