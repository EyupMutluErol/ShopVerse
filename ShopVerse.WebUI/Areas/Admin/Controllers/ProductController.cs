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

        // ============================================================
        // LISTELEME (INDEX)
        // ============================================================
        public async Task<IActionResult> Index(string search, int? categoryId, string status)
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            // --- FİLTRELEME MANTIĞI ---
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
            }

            if (categoryId != null && categoryId > 0)
            {
                products = products.Where(x => x.CategoryId == categoryId).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                    products = products.Where(x => x.IsActive == true).ToList();
                else if (status == "passive")
                    products = products.Where(x => x.IsActive == false).ToList();
            }

            // --- VIEW BAG ---
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Status = status;
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");

            // --- MAPPING ---
            var model = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                IsActive = p.IsActive,
                IsHome = p.IsHome,
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? "Kategori Yok"
            }).ToList();

            return View(model);
        }

        // ============================================================
        // EKLEME (ADD)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Mapping: ViewModel (Nullable) -> Entity (Not Null)
                // Not: ModelState.IsValid olduğu için .Value kullanmak güvenlidir.
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price.Value, // decimal? -> decimal
                    DiscountRate = model.DiscountRate ?? 0, // int? -> int (Boşsa 0)
                    // Fiyat Hesaplama
                    PriceWithDiscount = model.Price.Value - (model.Price.Value * (model.DiscountRate ?? 0) / 100),
                    Stock = model.Stock.Value, // int? -> int
                    CategoryId = model.CategoryId.Value, // int? -> int
                    IsHome = model.IsHome,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                };

                // Resim Yükleme
                if (model.ImageFile != null)
                {
                    product.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "products");
                }
                else
                {
                    product.ImageUrl = "/img/no-image.png";
                }

                await _productService.AddAsync(product);
                TempData["AdminSuccess"] = "Ürün başarıyla eklendi.";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(model);
        }

        // ============================================================
        // GÜNCELLEME (UPDATE) - DÜZELTİLEN KISIM
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Entity -> ViewModel Mapping
            var model = new ProductUpdateViewModel
            {
                Id = id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountRate = product.DiscountRate,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ExistingImageUrl = product.ImageUrl, // Resmi göstermek için
                IsHome = product.IsHome,
                IsActive = product.IsActive,
            };

            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = await _productService.GetByIdAsync(model.Id);
                if (product == null)
                {
                    return NotFound();
                }

                // 1. Manuel Mapping ve Null Dönüşümleri (.Value veya ?? kullanımı)
                product.Name = model.Name;
                product.Description = model.Description;

                // HATA ÇÖZÜMÜ: Nullable tipleri normal tiplere çeviriyoruz.
                product.Price = model.Price.Value;
                product.Stock = model.Stock.Value;
                product.CategoryId = model.CategoryId.Value;
                product.DiscountRate = model.DiscountRate ?? 0;

                // Fiyat tekrar hesaplanmalı
                product.PriceWithDiscount = product.Price - (product.Price * product.DiscountRate / 100);

                product.IsHome = model.IsHome;
                product.IsActive = model.IsActive;
                product.UpdatedDate = DateTime.Now;

                // 2. Resim Güncelleme Kontrolü
                if (model.ImageFile != null)
                {
                    // Yeni resim yükle
                    product.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "products");
                }
                // Else: Resim seçilmediyse product.ImageUrl veritabanındaki haliyle kalır.

                await _productService.UpdateAsync(product);

                TempData["AdminSuccess"] = "Ürün başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            // Validasyon Hatası Varsa (Örn: Stok boşsa)
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name", model.CategoryId);

            // Modeli View'a geri gönder ki hataları göstersin
            return View(model);
        }

        // ============================================================
        // SİLME (DELETE)
        // ============================================================
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product != null)
            {
                // Resim temizliği (Opsiyonel)
                if (!string.IsNullOrEmpty(product.ImageUrl) && !product.ImageUrl.Contains("no-image"))
                {
                    var relativePath = product.ImageUrl.TrimStart('/');
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                await _productService.DeleteAsync(product);
                TempData["AdminSuccess"] = "Ürün başarıyla silindi.";
            }
            else
            {
                TempData["AdminError"] = "Ürün bulunamadı.";
            }
            return RedirectToAction("Index");
        }
    }
}