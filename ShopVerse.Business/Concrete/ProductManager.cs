using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.Business.Concrete;

public class ProductManager:GenericManager<Product>,IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository) : base(productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetFilteredProductsAsync(ProductFilterDto filter)
    {
        var products = await _productRepository.GetAllAsync();

        if (filter.CategoryIds != null && filter.CategoryIds.Any())
        {
            products = products.Where(x => filter.CategoryIds.Contains(x.CategoryId)).ToList();
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            products = products.Where(x => x.Name.ToLower().Contains(filter.Search.ToLower())).ToList();
        }

        if (filter.MinPrice.HasValue)
        {
            products = products.Where(x => x.Price >= filter.MinPrice.Value).ToList();
        }

        if (filter.MaxPrice.HasValue)
        {
            products = products.Where(x => x.Price <= filter.MaxPrice.Value).ToList();
        }

        switch (filter.SortOrder)
        {
            case "price_asc":
                products = products.OrderBy(x => x.Price).ToList();
                break;
            case "price_desc":
                products = products.OrderByDescending(x => x.Price).ToList();
                break;
            case "name_asc":
                products = products.OrderBy(x => x.Name).ToList();
                break;
            case "name_desc":
                products = products.OrderByDescending(x => x.Name).ToList();
                break;
            default:
                products = products.OrderByDescending(x => x.Id).ToList();
                break;
        }

        return products;
    }
}
