using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? categoryId,string search)
        {
            ViewData["Title"] = "Tüm Ürünler";
            List<Product> products;

            if(categoryId.HasValue)
            {
                products = await _productService.GetAllAsync(x=>x.CategoryId == categoryId.Value);
                ViewBag.Title = "Kategori Ürünleri";
            }
            else if(!string.IsNullOrEmpty(search))
            {
                products = await _productService.GetAllAsync(x=>x.Name.Contains(search));
                ViewBag.Title = $"'{search}' için arama sonuçları";
            }
            else
            {
                products = await _productService.GetAllAsync();
            }
            return View(products);
        }





        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
