using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfProductRepository : EfGenericRepository<Product>, IProductRepository
{
    private readonly ShopVerseContext _context;
    public EfProductRepository(ShopVerseContext context) : base(context)
    {
        _context = context;
    }

    public int GetProductCount()
    {
        return _context.Products.Count();
    }
    public List<Product> GetCriticalStock(int threshold)
    {
        return _context.Products
            .Where(x => x.Stock <= threshold && x.Stock > 0)
            .OrderBy(x => x.Stock)
            .ToList();
    }

    public List<BestSellerDto> GetBestSellers(int count)
    {
        var startDate = DateTime.Now.AddDays(-30);

        var query = from od in _context.OrderDetails
                    join o in _context.Orders on od.OrderId equals o.Id
                    join p in _context.Products on od.ProductId equals p.Id

                    // Onaylanmış, Kargoda veya Teslim Edilmiş olmalı.
                    where o.OrderDate >= startDate &&
                          (o.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Approved ||
                           o.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Shipped ||
                           o.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Delivered)

                    group od by new { p.Id, p.Name, p.ImageUrl, p.Price, p.Stock } into g

                    select new BestSellerDto
                    {
                        ProductId = g.Key.Id,
                        ProductName = g.Key.Name,
                        ImageUrl = g.Key.ImageUrl,
                        Price = g.Key.Price,
                        Stock = g.Key.Stock,
                        SalesCount = g.Sum(x => x.Quantity)
                    };

        return query.OrderByDescending(x => x.SalesCount)
                    .Take(count)
                    .ToList();
    }
}
