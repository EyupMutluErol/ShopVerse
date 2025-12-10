using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class ProductManager:GenericManager<Product>,IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository) : base(productRepository)
    {
        _productRepository = productRepository;
    }
}
