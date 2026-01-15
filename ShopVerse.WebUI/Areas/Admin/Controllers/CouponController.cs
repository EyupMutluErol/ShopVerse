using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models; // ViewModel namespace'i
using System;
using System.Threading.Tasks;

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
            await PopulateDropdowns(); // Dropdownları doldur
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. ViewModel -> Entity Dönüşümü (Mapping)
                var coupon = new Coupon
                {
                    Code = model.Code.ToUpper(), // Kodları her zaman büyük harf yap
                    IsPercentage = model.IsPercentage,

                    // ViewModel'de Validasyon olduğu için .Value güvenlidir
                    DiscountAmount = model.DiscountAmount.Value,
                    MinCartAmount = model.MinCartAmount.Value,
                    ExpirationDate = model.ExpirationDate.Value,

                    // Dropdown boş gelirse null olur (Entity'de int? olduğu için sorun yok)
                    CategoryId = model.CategoryId,

                    // String boş gelirse null olarak kaydet (FK hatası almamak için)
                    UserId = string.IsNullOrEmpty(model.UserId) ? null : model.UserId,

                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                // 2. Ekstra Kontrol: Kod daha önce kullanılmış mı?
                var existingCoupon = _couponService.GetByCode(coupon.Code);
                if (existingCoupon != null)
                {
                    ModelState.AddModelError("Code", "Bu kupon kodu zaten mevcut.");
                    await PopulateDropdowns(); // Hata olduğu için dropdownları tekrar doldur
                    return View(model);
                }

                // 3. Kayıt
                await _couponService.AddAsync(coupon);
                TempData["AdminSuccess"] = "Kupon başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            // Validasyon hatası varsa (ModelState Invalid)
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

        // Dropdown verilerini dolduran yardımcı metot (Kod tekrarını önlemek için)
        private async Task PopulateDropdowns()
        {
            // Kategoriler
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;

            // Üyeler (Sadece 'Member' rolündekiler)
            var members = await _userManager.GetUsersInRoleAsync("Member");
            ViewBag.Users = members;
        }
    }
}