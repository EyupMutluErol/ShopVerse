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
            var products = await _productService.GetAllAsync();
            var orders = await _orderService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            ViewBag.ProductCount = products.Count;
            ViewBag.OrderCount = orders.Count;
            ViewBag.CategoryCount = categories.Count;
            ViewBag.UserCount = _userManager.Users.Count();
            ViewBag.PendingOrders = orders.Count(x => x.OrderStatus == ShopVerse.Entities.Enums.OrderStatus.Pending);

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
