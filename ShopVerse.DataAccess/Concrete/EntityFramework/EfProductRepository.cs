using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

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
}
