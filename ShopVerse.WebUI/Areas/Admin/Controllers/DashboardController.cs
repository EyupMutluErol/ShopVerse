using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

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

        public DashboardController(IProductService productService, IOrderService orderService, ICategoryService categoryService, UserManager<AppUser> userManager)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. GENEL İSTATİSTİKLER
            // (Business katmanındaki manager metotlarını çağırıyoruz)
            ViewBag.ProductCount = _productService.GetProductCount();
            ViewBag.OrderCount = _orderService.GetTotalOrderCount();
            ViewBag.TotalTurnover = _orderService.GetTotalTurnover();

            // Kategori Sayısı
            var categories = await _categoryService.GetAllAsync();
            ViewBag.CategoryCount = categories.Count;

            // Bekleyen Siparişler
            var allOrders = await _orderService.GetAllAsync();
            ViewBag.PendingOrders = allOrders.Count(x => x.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Pending);

            // Kullanıcı Sayısı (Admin olmayanlar)
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

            // 2. TABLO VERİLERİ
            // Kritik Stok (20'den az)
            ViewBag.CriticalProducts = _productService.GetCriticalStock(20);

            // En Çok Satanlar (İlk 5)
            ViewBag.BestSellers = _productService.GetBestSellers(5);

            // 3. GRAFİK VERİSİ (YENİ - SON 6 AY)
            // Bu veriyi View tarafında JSON'a çevirip Chart.js ile çizdireceğiz
            var salesTrend = _orderService.GetSalesTrend(6);
            ViewBag.SalesTrend = salesTrend;

            return View();
        }
    }
}