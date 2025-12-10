using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete;

public class CategoryManager:GenericManager<Category>,ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryManager(ICategoryRepository categoryRepository):base(categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
}
