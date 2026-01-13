using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.Business.Abstract;

public interface IProductService:IGenericService<Product>
{
    Task<List<Product>> GetFilteredProductsAsync(ProductFilterDto filter);
    Task<decimal> CalculatePriceWithCampaignAsync(Product product);
    int GetProductCount();
}
