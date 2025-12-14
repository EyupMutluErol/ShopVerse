using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderService _orderService;

        public ProfileController(UserManager<AppUser> userManager, IOrderService orderService)
        {
            _userManager = userManager;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var orders = _orderService.GetOrdersByUserId(user.Id);

            var model = new UserProfileViewModel
            {
                User = user,
                Orders = orders
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AppUser AppUser , IFormFile? UserImage)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(UserImage != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(UserImage.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = resource + "/wwwroot/userimages/" + imageName;
                var directory = Path.GetDirectoryName(saveLocation);
                if(!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(saveLocation,FileMode.Create))
                {
                    await UserImage.CopyToAsync(stream);
                }

                user.ImageUrl = imageName;
            }

            user.Name = AppUser.Name;
            user.Surname = AppUser.Surname;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Icon"] = "success";
                TempData["Message"] = "Profil bilgileriniz güncellendi.";
                return RedirectToAction("Index");
            } else
            {
                TempData["Icon"] = "error";
                TempData["Message"] = "Bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
    }
}
