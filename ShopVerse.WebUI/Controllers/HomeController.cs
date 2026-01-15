using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;
using ShopVerse.WebUI.Models;
using System.Diagnostics;

namespace ShopVerse.WebUI.Controllers
{
    // [Authorize] kaldýrýldý, çünkü ana sayfayý herkes görebilmeli.
    // Sadece favori iþlemleri giriþ gerektiriyor.
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;
        private readonly UserManager<AppUser> _userManager;

        // YENÝ EKLENEN SERVÝS
        private readonly IFavoriteService _favoriteService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            ICategoryService categoryService,
            ICampaignService campaignService,
            ICouponService couponService,
            UserManager<AppUser> userManager,
            IFavoriteService favoriteService) // Constructor Injection
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _campaignService = campaignService;
            _couponService = couponService;
            _userManager = userManager;
            _favoriteService = favoriteService; // Atama
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
            // 5. GÜNCELLENEN: KÝÞÝYE ÖZEL KUPONLARI GETÝR
            // ============================================================

            // A. Giriþ yapmýþ kullanýcýyý bul (Yoksa null döner)
            var user = await _userManager.GetUserAsync(User);
            string? currentUserId = user?.Id;

            // B. Sorguyu güncelle: 
            // - Aktif olacak
            // - Süresi dolmamýþ olacak
            // - VE (UserId boþ olacak [Genel] VEYA UserId benim ID'm olacak [Özel])
            var activeCoupons = await _couponService.GetAllAsync(x =>
                x.IsActive &&
                x.ExpirationDate >= DateTime.Now &&
                (x.UserId == null || x.UserId == currentUserId)
            );

            // View tarafýnda banner alanýnda göstermek için ViewBag'e atýyoruz
            ViewBag.ActiveCoupons = activeCoupons;

            // ============================================================
            // 6. FAVORÝLERÝ GETÝR (YENÝ EKLENEN KISIM)
            // ============================================================
            if (user != null)
            {
                // Giriþ yapmýþsa favorilerini çekiyoruz
                var userFavorites = await _favoriteService.GetFavoritesWithProductsAsync(user.Id);
                // Sadece ProductId'leri liste olarak View'a gönderiyoruz
                ViewBag.FavoriteProductIds = userFavorites.Select(x => x.ProductId).ToList();
            }
            else
            {
                // Giriþ yapmamýþsa boþ liste gönderiyoruz (Hata almamak için)
                ViewBag.FavoriteProductIds = new List<int>();
            }

            // 7. ViewModel Oluþturma
            var model = new HomeViewModel
            {
                FeaturedProducts = products,
                Categories = categories,
                ActiveCampaigns = activeCampaigns.ToList()
            };

            // 8. Filtrelerin View'da korunmasý için ViewData atamalarý
            ViewData["CurrentSearch"] = search;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SelectedCategories"] = categoryIds;
            ViewData["SortOrder"] = sortOrder;

            return View(model);
        }

        public IActionResult About()
        {
            return View();
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