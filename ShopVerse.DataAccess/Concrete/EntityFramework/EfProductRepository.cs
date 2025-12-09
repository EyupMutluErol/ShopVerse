using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfProductRepository : EfGenericRepository<Product>, IProductRepository
{
    public EfProductRepository(ShopVerseContext context) : base(context)
    {
    }
}
