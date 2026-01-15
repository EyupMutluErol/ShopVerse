using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using Microsoft.AspNetCore.Identity; // EKLENDİ

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
        private readonly UserManager<AppUser> _userManager; // EKLENDİ

        public CouponController(
            ICouponService couponService,
            ICategoryService categoryService,
            UserManager<AppUser> userManager) // EKLENDİ
        {
            _couponService = couponService;
            _categoryService = categoryService;
            _userManager = userManager; // ATANDI
        }

        public async Task<IActionResult> Index()
        {
            var values = await _couponService.GetAllAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // 1. Kategorileri Çek
            ViewBag.Categories = await _categoryService.GetAllAsync();

            // 2. YENİ: Üyeleri Çek (Admin olmayan, sadece 'Member' rolündekiler)
            // Not: Eğer çok fazla üye varsa bu yöntem yavaşlatabilir, select2 veya searchbox gerekebilir.
            // Şimdilik basit liste olarak alıyoruz.
            var members = await _userManager.GetUsersInRoleAsync("Member");
            ViewBag.Users = members;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            // 1. FIX: Eğer seçim yapılmadıysa gelen "" değerini NULL yap.
            if (string.IsNullOrEmpty(coupon.UserId))
            {
                coupon.UserId = null;
            }

            // 2. FIX: Eğer kategori seçilmediyse gelen 0 veya "" değerini NULL yap.
            if (coupon.CategoryId == 0)
            {
                coupon.CategoryId = null;
            }

            // 3. FIX: Navigation Property'leri validasyondan çıkar
            // (Formdan AppUser veya Category nesnesi gelmiyor, sadece ID'leri geliyor, bu yüzden hata verebilir)
            ModelState.Remove("AppUser");
            ModelState.Remove("Category");

            // --- Standart İşlemler ---
            coupon.Code = coupon.Code.ToUpper();

            if (coupon.ExpirationDate < DateTime.Now)
            {
                ModelState.AddModelError("ExpirationDate", "Son kullanma tarihi geçmiş olamaz.");
            }

            if (ModelState.IsValid)
            {
                await _couponService.AddAsync(coupon);
                return RedirectToAction("Index");
            }

            // Hata varsa sayfayı tekrar doldur
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Users = await _userManager.GetUsersInRoleAsync("Member");

            return View(coupon);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var value = await _couponService.GetByIdAsync(id);
            if (value != null)
            {
                await _couponService.DeleteAsync(value);
                TempData["Success"] = "Kupon başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }
    }
}