using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Enums;

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
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;

        public DashboardController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<AppUser> userManager,
            ICampaignService campaignService,
            ICouponService couponService)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _userManager = userManager;
            _campaignService = campaignService;
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // --- VERİLERİ ÇEK (SİLİNMİŞLERİ HARİÇ TUT) ---
            // GetAllAsync() muhtemelen tüm veriyi getiriyor. 
            // IsDeleted kontrolü ekleyerek 'Hayalet Veri' sorununu çözüyoruz.

            var allData = await _orderService.GetAllAsync();
            var allOrders = allData.Where(x => !x.IsDeleted).ToList(); // SADECE AKTİF SİPARİŞLER

            var productData = await _productService.GetAllAsync();
            var products = productData.Where(x => !x.IsDeleted).ToList();

            var categoryData = await _categoryService.GetAllAsync();
            var categories = categoryData.Where(x => !x.IsDeleted).ToList();

            var campaignData = await _campaignService.GetAllAsync();
            var campaigns = campaignData.Where(x => !x.IsDeleted).ToList();

            var couponData = await _couponService.GetAllAsync();
            var coupons = couponData.Where(x => !x.IsDeleted).ToList();


            // --- 1. SİPARİŞ KARTI (MAVİ KUTU - SADECE BEKLEYENLER) ---
            var pendingCount = allOrders.Count(x => x.OrderStatus == OrderStatus.Pending);

            ViewBag.PendingOrdersCount = pendingCount;
            ViewBag.PendingOrders = pendingCount;


            // --- 2. TOPLAM SİPARİŞ KARTI (YEŞİL KUTU) ---
            ViewBag.TotalOrders = allOrders.Count;


            // --- 3. AKTİF SİPARİŞLER (SARI KUTU) ---
            var activeOrders = allOrders.Count(x =>
                x.OrderStatus == OrderStatus.Pending ||
                x.OrderStatus == OrderStatus.Approved ||
                x.OrderStatus == OrderStatus.Shipped);
            ViewBag.ActiveOrders = activeOrders;


            // --- 4. NET CİRO HESABI (SİYAH KUTU) ---
            // İptal ve İade hariç tutuluyor.
            var netRevenue = allOrders
                .Where(x => x.OrderStatus != OrderStatus.Canceled && x.OrderStatus != OrderStatus.Refunded)
                .Sum(x => x.TotalPrice);

            ViewBag.TotalTurnover = netRevenue;
            ViewBag.TotalRevenue = netRevenue;


            // --- 5. TESLİM EDİLENLER ---
            ViewBag.DeliveredCount = allOrders.Count(x => x.OrderStatus == OrderStatus.Delivered);


            // --- 6. DİĞER İSTATİSTİKLER ---

            ViewBag.ProductCount = products.Count;
            ViewBag.CategoryCount = categories.Count;

            // Üye Sayısı
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

            // Kampanya ve Kuponlar (Aktif ve süresi dolmamış)
            ViewBag.CampaignCount = campaigns.Count(x => x.IsActive && x.EndDate >= DateTime.Now);
            ViewBag.CouponCount = coupons.Count(x => x.IsActive && x.ExpirationDate >= DateTime.Now);


            // --- 7. TABLO VERİLERİ ---

            // Kritik Stok (Stok < 20)
            var criticalProducts = products.Where(x => x.Stock < 20).ToList();
            ViewBag.CriticalProducts = criticalProducts;
            ViewBag.CriticalStockCount = criticalProducts.Count;

            // En Çok Satanlar (Service katmanında IsDeleted kontrolü yapıldığı varsayılıyor, yoksa burada da filtre gerekebilir)
            ViewBag.BestSellers = _productService.GetBestSellers(5);

            // Satış Trendi
            var salesTrend = _orderService.GetSalesTrend(6);
            ViewBag.SalesTrend = salesTrend;

            return View();
        }
    }
}