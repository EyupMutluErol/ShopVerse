using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var values = await _couponService.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _couponService.GetByIdAsync(id);
            if (value == null) return NotFound();
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            await _couponService.AddAsync(coupon);
            return Ok("Kupon eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Coupon coupon)
        {
            await _couponService.UpdateAsync(coupon);
            return Ok("Kupon güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _couponService.GetByIdAsync(id);
            if (value == null) return NotFound();
            await _couponService.DeleteAsync(value);
            return Ok("Kupon silindi");
        }
    }
}