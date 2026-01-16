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

        // ============================================================
        // LİSTELEME (INDEX)
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var values = await _couponService.GetAllAsync();
            return View(values);
        }

        // ============================================================
        // EKLEME (CREATE)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                var coupon = new Coupon
                {
                    Code = model.Code.ToUpper(),
                    IsPercentage = model.IsPercentage,
                    DiscountAmount = model.DiscountAmount ?? 0,
                    MinCartAmount = model.MinCartAmount ?? 0,
                    ExpirationDate = model.ExpirationDate ?? DateTime.Now.AddDays(7),

                    // Yeni Alanlar
                    MinProductPrice = model.MinProductPrice,
                    MaxProductPrice = model.MaxProductPrice,
                    CategoryId = model.CategoryId,
                    UserId = string.IsNullOrEmpty(model.UserId) ? null : model.UserId,

                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                // Kod kontrolü (Opsiyonel: Aynı kod var mı?)
                // var existing = _couponService.GetByCode(coupon.Code);
                // if(existing != null) { ... }

                await _couponService.AddAsync(coupon);
                TempData["AdminSuccess"] = "Kupon başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns();
            return View(model);
        }

        // ============================================================
        // GÜNCELLEME (UPDATE) - YENİ EKLENEN KISIM
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var coupon = await _couponService.GetByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            // Entity -> ViewModel Dönüşümü
            var model = new CouponUpdateViewModel
            {
                Id = coupon.Id,
                Code = coupon.Code,
                DiscountAmount = coupon.DiscountAmount,
                IsPercentage = coupon.IsPercentage,
                MinCartAmount = coupon.MinCartAmount,
                ExpirationDate = coupon.ExpirationDate,
                IsActive = coupon.IsActive,

                // İlişkiler ve Kısıtlamalar
                CategoryId = coupon.CategoryId,
                UserId = coupon.UserId,
                MinProductPrice = coupon.MinProductPrice,
                MaxProductPrice = coupon.MaxProductPrice
            };

            await PopulateDropdowns(); // Dropdown verilerini hazırla
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(CouponUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var coupon = await _couponService.GetByIdAsync(model.Id);
                if (coupon == null)
                {
                    return NotFound();
                }

                // Verileri Güncelle
                coupon.Code = model.Code.ToUpper();
                coupon.DiscountAmount = model.DiscountAmount ?? 0;
                coupon.IsPercentage = model.IsPercentage;
                coupon.MinCartAmount = model.MinCartAmount ?? 0;
                coupon.ExpirationDate = model.ExpirationDate ?? DateTime.Now.AddDays(7);
                coupon.IsActive = model.IsActive;

                coupon.CategoryId = model.CategoryId;
                coupon.UserId = string.IsNullOrEmpty(model.UserId) ? null : model.UserId;
                coupon.MinProductPrice = model.MinProductPrice;
                coupon.MaxProductPrice = model.MaxProductPrice;

                // Güncelleme Tarihi (Entity'de varsa)
                // coupon.UpdatedDate = DateTime.Now; 

                await _couponService.UpdateAsync(coupon);
                TempData["AdminSuccess"] = "Kupon başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            await PopulateDropdowns();
            return View(model);
        }

        // ============================================================
        // SİLME (DELETE)
        // ============================================================
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
        // YARDIMCI METOT (DROPDOWN)
        // ================================================================
        private async Task PopulateDropdowns()
        {
            // 1. Kategoriler (SelectList Olarak)
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            // 2. Üyeler (Liste Olarak - View'da foreach ile dönülüyor)
            var users = _userManager.Users.ToList();
            ViewBag.Users = users;
        }
    }
}