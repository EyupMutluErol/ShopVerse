using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfCategoryRepository : EfGenericRepository<Category>, ICategoryRepository
{
    public EfCategoryRepository(ShopVerseContext context) : base(context)
    {
    }
}
