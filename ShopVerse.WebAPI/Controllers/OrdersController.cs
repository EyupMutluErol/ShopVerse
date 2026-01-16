using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var values = await _orderService.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _orderService.GetByIdAsync(id);
            if (value == null) return NotFound();
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            await _orderService.AddAsync(order);
            return Ok("Sipariş oluşturuldu");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Order order)
        {
            await _orderService.UpdateAsync(order);
            return Ok("Sipariş güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _orderService.GetByIdAsync(id);
            if (value == null) return NotFound();
            await _orderService.DeleteAsync(value);
            return Ok("Sipariş silindi");
        }
    }
}