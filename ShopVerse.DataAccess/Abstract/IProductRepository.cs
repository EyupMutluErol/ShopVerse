using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.DataAccess.Abstract;

public interface IProductRepository:IGenericRepository<Product>
{
    int GetProductCount();
    List<Product> GetCriticalStock(int threshold);
    List<BestSellerDto> GetBestSellers(int count);
}
