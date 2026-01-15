using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopVerse.DataAccess.Concrete.Context; // Context'in doğru namespace'i
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ShopVerseContext _context;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, ShopVerseContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // --- LİSTELEME METOTLARI ---

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(userList);
        }

        public async Task<IActionResult> Admins()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var userList = new List<UserViewModel>();
            foreach (var user in admins)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(userList);
        }

        public async Task<IActionResult> Members()
        {
            var members = await _userManager.GetUsersInRoleAsync("Member");
            var userList = new List<UserViewModel>();
            foreach (var user in members)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin")) continue;

                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return View(userList);
        }

        // --- ROL ATAMA METOTLARI ---

        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new RoleAssignViewModel
            {
                UserId = user.Id,
                UserName = user.Name + " " + user.Surname,
                Roles = roles.Select(r => new RoleAssignItem
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Exists = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            foreach (var item in model.Roles)
            {
                if (item.Exists)
                {
                    await _userManager.AddToRoleAsync(user, item.RoleName);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            return isAdmin ? RedirectToAction("Admins") : RedirectToAction("Members");
        }

        // --- SİLME İŞLEMİ (GÜNCELLENMİŞ HALİ: SET NULL) ---

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Members");

            // 1. Kendi kendini silmeyi engelle
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id == user.Id)
            {
                TempData["Error"] = "Kendi hesabınızı silemezsiniz!";
                return RedirectToAction("Admins");
            }

            // Silinecek kişinin admin olup olmadığını önceden alalım (Yönlendirme için)
            bool wasAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // ====================================================================
            // GÜNCELLEME: Siparişleri SİLME, Kullanıcı İlişkisini BOŞA ÇIKAR (NULL YAP)
            // ====================================================================

            // 1. Bu kullanıcının siparişlerini bul
            var userOrders = _context.Orders.Where(x => x.AppUserId == user.Id).ToList();

            // 2. Siparişleri döngüye al ve UserId'yi NULL yap
            if (userOrders.Any())
            {
                foreach (var order in userOrders)
                {
                    order.AppUserId = null; // Siparişi sahipsiz bırak ama silme
                }

                // Değişiklikleri kaydet (Update işlemi yapar)
                await _context.SaveChangesAsync();
            }

            // ====================================================================

            // 3. Artık kullanıcıyı silebiliriz (İlişki koparıldığı için hata vermez)
            var result = await _userManager.DeleteAsync(user);

            // 4. Doğru Sayfaya Yönlendir
            if (result.Succeeded)
            {
                TempData["AdminSuccess"] = "Kullanıcı başarıyla silindi. Sipariş geçmişi korundu.";
                if (wasAdmin) return RedirectToAction("Admins");
                else return RedirectToAction("Members");
            }
            else
            {
                TempData["Error"] = "Silme işlemi sırasında bir hata oluştu.";
            }

            return RedirectToAction("Members");
        }
    }
}