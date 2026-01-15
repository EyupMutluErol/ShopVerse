using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using ShopVerse.WebUI.Utils;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ImageHelper _imageHelper;

        public ProductController(IProductService productService, ICategoryService categoryService, ImageHelper imageHelper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var model = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                IsHome = p.IsHome,
                CategoryName = categories.FirstOrDefault(c=>c.Id == p.CategoryId)?.Name ?? "Kategori Yok"
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(),"Id","Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel model)
        {
            if(ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price.Value,
                    DiscountRate = model.DiscountRate?? 0,
                    PriceWithDiscount = model.Price.Value - (model.Price.Value * (model.DiscountRate ?? 0) / 100),
                    Stock = model.Stock.Value,
                    CategoryId = model.CategoryId.Value,
                    IsHome = model.IsHome,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                };

                if(model.ImageFile != null)
                {
                    product.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "products");
                }
                else
                {
                    product.ImageUrl = "/img/no-image.png"; // Varsayılan resim
                }

                await _productService.AddAsync(product);
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            var model = new ProductUpdateViewModel
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountRate = product.DiscountRate,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ExistingImageUrl = product.ImageUrl,
                IsHome = product.IsHome,
                IsActive = product.IsActive,
            };

            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name",product.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateViewModel model)
        {
            if(ModelState.IsValid)
            {
                var product = await _productService.GetByIdAsync(model.Id);
                if (product == null)
                {  
                    return NotFound();
                }

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.DiscountRate = model.DiscountRate ?? 0;
                product.PriceWithDiscount = model.Price - (model.Price * (model.DiscountRate ?? 0) / 100);
                product.Stock = model.Stock;
                product.CategoryId = model.CategoryId;
                product.IsHome = model.IsHome;
                product.IsActive = model.IsActive;
                product.UpdatedDate = DateTime.Now;

                if(model.ImageFile != null)
                {
                    product.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "products");
                }

                await _productService.UpdateAsync(product);
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(),"Id","Name",model.CategoryId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product != null)
            {
                await _productService.DeleteAsync(product);
            } 
            return RedirectToAction("Index");
        }
    }
}
