using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName,string description)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = new AppRole
                {
                    Name = roleName,
                    Description = description 
                };

                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Rol başarıyla oluşturuldu.";
                }
                else
                {
                    TempData["Error"] = "Hata: " + string.Join(" ", result.Errors.Select(e => e.Description));
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id) 
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Hata: Rol ID bilgisi alınamadı.";
                return RedirectToAction("Index");
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["Error"] = "Hata: Silinmek istenen rol bulunamadı.";
                return RedirectToAction("Index");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Count > 0)
            {
                TempData["Error"] = $"'{role.Name}' rolünü kullanan {usersInRole.Count} kullanıcı var. Silmeden önce bu kullanıcıların rolünü değiştirmelisiniz.";
                return RedirectToAction("Index");
            }

            string roleNameLower = role.Name?.ToLower().Trim();
            if (roleNameLower == "admin" || roleNameLower == "member")
            {
                TempData["Error"] = $"Sistem için kritik olan '{role.Name}' rolü silinemez.";
                return RedirectToAction("Index");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "Rol başarıyla silindi.";
            }
            else
            {
                string errors = string.Join(" ", result.Errors.Select(e => e.Description));
                TempData["Error"] = $"Silme işlemi başarısız: {errors}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string roleId,string description)
        {
            if(string.IsNullOrEmpty(roleId))
            {
                TempData["Error"] = "Rol bulunamadı.";
                return RedirectToAction("Index");
            }

            var role = await _roleManager.FindByIdAsync(roleId);

            if(role != null)
            {
                role.Description = description;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Rol başarıyla güncellendi.";
                }
                else
                {
                    TempData["Error"] = "Güncelleme hatası.";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
