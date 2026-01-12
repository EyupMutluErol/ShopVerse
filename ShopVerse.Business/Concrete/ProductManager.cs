using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.Business.Concrete;

public class ProductManager : GenericManager<Product>, IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository) : base(productRepository)
    {
        _productRepository = productRepository;
    }

    public override async Task AddAsync(Product product)
    {
        CalculateDiscountedPrice(product);
        await _productRepository.AddAsync(product);
    }

    public override async Task UpdateAsync(Product product)
    {
        CalculateDiscountedPrice(product);
        await _productRepository.UpdateAsync(product);
    }

    private void CalculateDiscountedPrice(Product product)
    {
        if (product.DiscountRate > 0)
        {
            product.PriceWithDiscount = product.Price - (product.Price * product.DiscountRate / 100m);
        }
        else
        {
            product.PriceWithDiscount = product.Price;
        }
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
            products = products.Where(x => x.PriceWithDiscount >= filter.MinPrice.Value).ToList();
        }

        if (filter.MaxPrice.HasValue)
        {
            products = products.Where(x => x.PriceWithDiscount <= filter.MaxPrice.Value).ToList();
        }

        switch (filter.SortOrder)
        {
            case "price_asc":
                products = products.OrderBy(x => x.PriceWithDiscount).ToList();
                break;
            case "price_desc":
                products = products.OrderByDescending(x => x.PriceWithDiscount).ToList();
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