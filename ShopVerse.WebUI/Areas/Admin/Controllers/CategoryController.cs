using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using ShopVerse.WebUI.Utils;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ImageHelper _imageHelper;

        public CategoryController(ICategoryService categoryService,ImageHelper imageHelper)
        {
            _categoryService = categoryService;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();

            var model = categories.Select(x => new CategoryListViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                ImageUrl = x.ImageUrl,
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryAddViewModel model)
        {
            if(ModelState.IsValid)
            {
                var category = new Category
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                if(model.ImageFile != null)
                {
                    category.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "categories");
                }

                await _categoryService.AddAsync(category);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            var model = new CategoryUpdateViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ExistingImageUrl = category.ImageUrl,
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CategoryUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = await _categoryService.GetByIdAsync(model.Id);
                if(category == null)
                {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.UpdatedDate = DateTime.Now;

                if(model.ImageFile != null)
                {
                    category.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "categories");
                }

                await _categoryService.UpdateAsync(category);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if(category != null)
            {
                await _categoryService.DeleteAsync(category);
            }
            return RedirectToAction("Index");
        }
    }
}
