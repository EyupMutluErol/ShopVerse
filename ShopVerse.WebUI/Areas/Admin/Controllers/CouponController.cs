using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using System.Threading.Tasks;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        private readonly ICategoryService _categoryService;

        public CouponController(ICouponService couponService, ICategoryService categoryService)
        {
            _couponService = couponService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var values = await _couponService.GetAllAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            // Kupon kodu büyük harfe çevrilsin
            coupon.Code = coupon.Code.ToUpper();

            // Tarih kontrolü (Geçmiş tarih seçilmesin)
            if (coupon.ExpirationDate < DateTime.Now)
            {
                ModelState.AddModelError("ExpirationDate", "Son kullanma tarihi geçmiş olamaz.");
            }

            if (ModelState.IsValid)
            {
                await _couponService.AddAsync(coupon);
                return RedirectToAction("Index");
            }
            return View(coupon);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var value = await _couponService.GetByIdAsync(id);
            await _couponService.DeleteAsync(value);
            return RedirectToAction("Index");
        }

        // Edit işlemleri Create ile çok benzer, şimdilik Create ve Index yeterli olacaktır.
        // İstersen Edit metodunu da ekleyebiliriz.
    }
}