using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList için GEREKLİ
using Microsoft.AspNetCore.Identity;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<AppUser> _userManager;

        public CouponController(
            ICouponService couponService,
            ICategoryService categoryService,
            UserManager<AppUser> userManager)
        {
            _couponService = couponService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _couponService.GetAllAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns(); // Dropdownları SelectList formatında doldur
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. ViewModel -> Entity Dönüşümü
                var coupon = new Coupon
                {
                    Code = model.Code.ToUpper(),
                    IsPercentage = model.IsPercentage,
                    DiscountAmount = model.DiscountAmount ?? 0,
                    MinCartAmount = model.MinCartAmount ?? 0,
                    ExpirationDate = model.ExpirationDate ?? DateTime.Now.AddDays(7),

                    // Yeni eklenen alanlar
                    MinProductPrice = model.MinProductPrice,
                    MaxProductPrice = model.MaxProductPrice,
                    CategoryId = model.CategoryId,
                    UserId = string.IsNullOrEmpty(model.UserId) ? null : model.UserId,

                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                // 2. Ekstra Kontrol: Kod daha önce kullanılmış mı?
                // Not: Servisinizde GetByCode yoksa bu kontrolü kaldırabilir veya ekleyebilirsiniz.
                // var existingCoupon = _couponService.GetByCode(coupon.Code);
                // if (existingCoupon != null) ...

                // 3. Kayıt
                await _couponService.AddAsync(coupon);
                TempData["AdminSuccess"] = "Kupon başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            // Validasyon hatası varsa dropdownları tekrar doldur
            await PopulateDropdowns();
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var value = await _couponService.GetByIdAsync(id);
            if (value != null)
            {
                await _couponService.DeleteAsync(value);
                TempData["AdminSuccess"] = "Kupon başarıyla silindi.";
            }
            else
            {
                TempData["Error"] = "Kupon bulunamadı.";
            }
            return RedirectToAction("Index");
        }

        // ================================================================
        // ÖNEMLİ DÜZELTME: Dropdown Doldurma Yardımcısı
        // ================================================================
        private async Task PopulateDropdowns()
        {
            // 1. Kategoriler: List<Category> yerine SelectList'e çeviriyoruz
            var categories = await _categoryService.GetAllAsync();

            // ViewBag.Categories artık "IEnumerable<SelectListItem>" türünde olacak
            // "Id": Value (Arka planda tutulan değer)
            // "Name": Text (Ekranda görünen metin)
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            // 2. Üyeler
            // Not: UserManager.Users IQueryable döner, .ToList() ile çekiyoruz.
            // Eğer rol bazlı çekmek isterseniz GetUsersInRoleAsync kullanabilirsiniz.
            var users = _userManager.Users.ToList();

            // Eğer View tarafında foreach ile dönecekseniz List olarak kalabilir,
            // ama asp-items kullanacaksanız bunu da SelectList yapmalısınız.
            // Sizin View kodunuzda 'foreach' olduğu için List olarak bırakıyorum:
            ViewBag.Users = users;
        }
    }
}