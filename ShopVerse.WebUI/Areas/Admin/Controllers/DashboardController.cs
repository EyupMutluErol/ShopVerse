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

        public async Task<IActionResult> Index()
        {
            // ------------------------------------------------------------
            // YENİ METOTLARLA OPTİMİZE EDİLMİŞ VERİ ÇEKME
            // ------------------------------------------------------------

            // 1. Toplam Ürün Sayısı (Tüm listeyi çekmek yerine sadece sayıyı alıyoruz)
            ViewBag.ProductCount = _productService.GetProductCount();

            // 2. Toplam Sipariş Sayısı
            ViewBag.OrderCount = _orderService.GetTotalOrderCount();

            // 3. Toplam Ciro (YENİ ÖZELLİK)
            ViewBag.TotalTurnover = _orderService.GetTotalTurnover();

            // ------------------------------------------------------------
            // DİĞER İSTATİSTİKLER
            // ------------------------------------------------------------

            // Kategori Sayısı
            var categories = await _categoryService.GetAllAsync();
            ViewBag.CategoryCount = categories.Count;

            // Bekleyen Siparişler
            // (Bunu da Business katmanına özel metot olarak ekleyebiliriz ama şimdilik filtreleyerek alıyoruz)
            var allOrders = await _orderService.GetAllAsync();
            ViewBag.PendingOrders = allOrders.Count(x => x.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Pending);

            // Kullanıcı Sayısı (Admin olmayan Üyeler)
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

            return View();
        }
    }
}