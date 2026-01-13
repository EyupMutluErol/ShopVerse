using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Abstract;

public interface IProductRepository:IGenericRepository<Product>
{
    int GetProductCount();
}
