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
        // Bu GenericManager'da yok, DAL'daki özel metodu çağırıyoruz
        return _favoriteRepository.GetFavoritesWithProducts(userId);
    }

    public async Task<Favorite> GetByProductAndUserAsync(int productId, string userId)
    {
        // GenericManager'da "Get(filter)" olmadığı için "GetAllAsync" kullanıp ilkini alıyoruz.
        var favorites = await base.GetAllAsync(x => x.ProductId == productId && x.UserId == userId);
        return favorites.FirstOrDefault();
    }

    public async Task<int> GetCountByUserIdAsync(string userId)
    {
        // GenericManager'daki GetAllAsync'i kullanıyoruz
        var favorites = await base.GetAllAsync(x => x.UserId == userId);
        return favorites.Count;
    }

    public async Task ToggleFavoriteAsync(string userId, int productId)
    {
        // 1. Önce bu kayıt var mı kontrol et (Kendi yazdığımız metodu kullanıyoruz)
        var existingFavorite = await GetByProductAndUserAsync(productId, userId);

        if (existingFavorite != null)
        {
            // 2. Varsa SİL -> GenericManager'daki DeleteAsync çalışır
            await base.DeleteAsync(existingFavorite);
        }
        else
        {
            // 3. Yoksa EKLE -> GenericManager'daki AddAsync çalışır
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
