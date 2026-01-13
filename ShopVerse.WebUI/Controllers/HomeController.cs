using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Dtos;
using ShopVerse.WebUI.Models;
using System.Diagnostics;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;


        public HomeController(ILogger<HomeController> logger,IProductService productService, ICategoryService categoryService, ICampaignService campaignService, ICouponService couponService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _campaignService = campaignService;
            _couponService = couponService;
        }

        public async Task<IActionResult> Index(List<int>? categoryIds, decimal? minPrice, decimal? maxPrice, string search, string sortOrder)
        {
            // 1. Kategorileri Getir
            var categories = await _categoryService.GetAllAsync();

            // 2. Filtreleme Ayarlarý
            var filterDto = new ProductFilterDto
            {
                CategoryIds = categoryIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Search = search,
                SortOrder = sortOrder
            };

            // 3. Filtrelenmiþ Ürünleri Getir
            var products = await _productService.GetFilteredProductsAsync(filterDto);

            // 4. Aktif Kampanyalarý Getir
            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                  x.IsActive &&
                  x.StartDate <= DateTime.Now &&
                  x.EndDate >= DateTime.Now
            );

            // ============================================================
            // 5. YENÝ: AKTÝF KUPONLARI GETÝR (ViewBag'e Atýyoruz)
            // ============================================================
            // Sadece aktif ve tarihi geçmemiþ kuponlarý çekiyoruz.
            var activeCoupons = await _couponService.GetAllAsync(x => x.IsActive && x.ExpirationDate >= DateTime.Now);

            // View tarafýnda ürün kartlarýnda kontrol etmek için ViewBag'e atýyoruz
            ViewBag.ActiveCoupons = activeCoupons;

            // 6. ViewModel Oluþturma
            var model = new HomeViewModel
            {
                FeaturedProducts = products,
                Categories = categories,
                ActiveCampaigns = activeCampaigns.ToList()
            };

            // 7. Filtrelerin View'da korunmasý için ViewData atamalarý
            ViewData["CurrentSearch"] = search;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SelectedCategories"] = categoryIds;
            ViewData["SortOrder"] = sortOrder;

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
