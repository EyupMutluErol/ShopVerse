using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Enums;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderService _orderService;

        public CheckoutController(UserManager<AppUser> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            if(cart.Count == 0)
            {
                TempData["Icon"] = "error";
                TempData["Message"] = "Sepetiniz boş, sipariş veremezsiniz";
                return RedirectToAction("Index","Cart");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            if(cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var order = new Order
                {
                    OrderNumber = "SP-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    AppUserId = user.Id,

                    // Formdan gelen adres bilgileri
                    FullName = model.FullName,
                    AddressLine = model.AddressLine,
                    City = model.City,
                    District = model.District,
                    PhoneNumber = model.PhoneNumber,

                    // Sepet toplamı
                    TotalPrice = cart.Sum(x => x.TotalPrice),

                    // Detaylar listesi
                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var item in cart)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.Product.Id,
                        Price = item.Product.Price,
                        Quantity = item.Quantity
                    });
                }

                await _orderService.AddAsync(order);

                HttpContext.Session.Remove("Cart"); // Sepeti temizle

                TempData["Icon"] = "success";
                TempData["Message"] = "Siparişiniz başarıyla alındı!";

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
