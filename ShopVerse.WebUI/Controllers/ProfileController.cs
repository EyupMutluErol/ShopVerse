using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Concrete.Context; // Hesap silerken Context kullanımı devam ediyor (Hızlı çözüm)
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

            // Kullanıcının siparişlerini getir
            var orders = _orderService.GetOrdersByUserId(user.Id);

            // Siparişleri tarihe göre yeniden eskiye (son sipariş en üstte) sıralayalım
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
            user.PhoneNumber = AppUser.PhoneNumber; // Telefon da güncellensin

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

        // ============================================================
        // SİPARİŞ İPTAL VE İADE AKSİYONLARI (MANAGER KONTROLLÜ)
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                // Business katmanındaki metodu çağırıyoruz.
                // Eğer sipariş Admin tarafından onaylanmışsa, Manager hata fırlatacak.
                await _orderService.CancelOrderAsync(id);
                TempData["UserSuccess"] = "Siparişiniz başarıyla iptal edildi.";
            }
            catch (Exception ex)
            {
                // Manager'dan gelen hatayı (Örn: "Sipariş onaylandığı için iptal edilemez")
                // kullanıcıya gösteriyoruz.
                TempData["UserError"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ReturnOrder(int id)
        {
            try
            {
                // Business katmanındaki metodu çağırıyoruz.
                // 3 gün kuralı ve teslimat kontrolü burada yapılıyor.
                await _orderService.ReturnOrderAsync(id);
                TempData["UserSuccess"] = "İade talebiniz alındı. Süreç başlatıldı.";
            }
            catch (Exception ex)
            {
                TempData["UserError"] = ex.Message; // Örn: "İade süresi dolmuştur."
            }
            return RedirectToAction("Index");
        }

        // ============================================================
        // HESAP SİLME
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            // 1. Siparişlerle olan ilişkiyi kopar (Sipariş geçmişi silinmez, anonim olur)
            var userOrders = _context.Orders.Where(x => x.AppUserId == user.Id).ToList();
            if (userOrders.Any())
            {
                foreach (var order in userOrders)
                {
                    order.AppUserId = null;
                }
                await _context.SaveChangesAsync();
            }

            // 2. Oturumu Kapat
            await _signInManager.SignOutAsync();

            // 3. Kullanıcıyı Sil
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }
    }
}