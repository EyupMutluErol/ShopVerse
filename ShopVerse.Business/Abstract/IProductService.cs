using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.Business.Abstract;

public interface IProductService:IGenericService<Product>
{
    Task<List<Product>> GetFilteredProductsAsync(ProductFilterDto filter);
    Task<decimal> CalculatePriceWithCampaignAsync(Product product);
    int GetProductCount();
    List<Product> GetCriticalStock(int threshold = 20);
    List<BestSellerDto> GetBestSellers(int count = 5);
}
