using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Concrete.Context; 
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOrderService _orderService;
        private readonly ShopVerseContext _context;

        public ProfileController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOrderService orderService,
            ShopVerseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var orders = _orderService.GetOrdersByUserId(user.Id);

            orders = orders.OrderByDescending(x => x.CreatedDate).ToList();

            var model = new UserProfileViewModel
            {
                User = user,
                Orders = orders
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUser AppUser, IFormFile? UserImage)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (UserImage != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(UserImage.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = Path.Combine(resource, "wwwroot/userimages", imageName);

                var directory = Path.GetDirectoryName(saveLocation);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(saveLocation, FileMode.Create))
                {
                    await UserImage.CopyToAsync(stream);
                }

                user.ImageUrl = imageName;
            }

            user.Name = AppUser.Name;
            user.Surname = AppUser.Surname;
            user.PhoneNumber = AppUser.PhoneNumber; 

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["UserSuccess"] = "Profil bilgileriniz güncellendi.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["UserError"] = "Bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                
                await _orderService.CancelOrderAsync(id);
                TempData["UserSuccess"] = "Siparişiniz başarıyla iptal edildi.";
            }
            catch (Exception ex)
            {
               
                TempData["UserError"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReturnOrder(int id)
        {
            try
            {
               
                await _orderService.ReturnOrderAsync(id);
                TempData["UserSuccess"] = "İade talebiniz alındı. Süreç başlatıldı.";
            }
            catch (Exception ex)
            {
                TempData["UserError"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

      

        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            var userOrders = _context.Orders.Where(x => x.AppUserId == user.Id).ToList();
            if (userOrders.Any())
            {
                foreach (var order in userOrders)
                {
                    order.AppUserId = null;
                }
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }
    }
}