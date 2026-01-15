using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Enums; // OrderStatus için gerekli

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<AppUser> _userManager;

        // YENİ EKLENEN SERVİSLER
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;

        public DashboardController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<AppUser> userManager,
            ICampaignService campaignService, // Yeni
            ICouponService couponService)     // Yeni
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _userManager = userManager;
            _campaignService = campaignService; // Atama
            _couponService = couponService;     // Atama
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // --- 1. GENEL İSTATİSTİKLER ---

            // Ürün ve Sipariş Sayıları (Business Katmanından)
            ViewBag.ProductCount = _productService.GetProductCount();
            ViewBag.OrderCount = _orderService.GetTotalOrderCount();
            ViewBag.TotalTurnover = _orderService.GetTotalTurnover();

            // Kategori Sayısı
            var categories = await _categoryService.GetAllAsync();
            ViewBag.CategoryCount = categories.Count;

            // Bekleyen Siparişler
            var allOrders = await _orderService.GetAllAsync();
            ViewBag.PendingOrders = allOrders.Count(x => x.OrderStatus == OrderStatus.Pending);

            // Üye Sayısı (Admin Olmayanlar)
            // Not: Büyük veride bu yöntem yavaş çalışabilir, ileride count sorgusu ile optimize edilebilir.
            var members = await _userManager.GetUsersInRoleAsync("Member");
            int realMemberCount = 0;
            foreach (var member in members)
            {
                if (!await _userManager.IsInRoleAsync(member, "Admin"))
                {
                    realMemberCount++;
                }
            }
            ViewBag.UserCount = realMemberCount;

            // --- YENİ EKLENENLER (Kampanya ve Kupon) ---

            // Aktif Kampanyalar
            var campaigns = await _campaignService.GetAllAsync();
            // Sadece aktif ve süresi dolmamışları sayıyoruz
            ViewBag.CampaignCount = campaigns.Count(x => x.IsActive && x.EndDate >= DateTime.Now);

            // Aktif Kuponlar
            var coupons = await _couponService.GetAllAsync();
            // Sadece aktif ve süresi dolmamışları sayıyoruz
            ViewBag.CouponCount = coupons.Count(x => x.IsActive && x.ExpirationDate >= DateTime.Now);


            // --- 2. TABLO VERİLERİ ---

            // Kritik Stok Listesi (Stok < 20)
            var criticalProducts = _productService.GetCriticalStock(20);
            ViewBag.CriticalProducts = criticalProducts;

            // Kritik Stok Sayısı (Dashboard kartı için gerekli)
            ViewBag.CriticalStockCount = criticalProducts.Count;

            // En Çok Satanlar (İlk 5)
            ViewBag.BestSellers = _productService.GetBestSellers(5);


            // --- 3. GRAFİK VERİSİ (SON 6 AY) ---
            var salesTrend = _orderService.GetSalesTrend(6);
            ViewBag.SalesTrend = salesTrend;

            return View();
        }
    }
}