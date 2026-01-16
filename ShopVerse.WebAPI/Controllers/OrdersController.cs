using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;

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
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetDetails(int id)
        {
            var order = _orderService.GetOrderWithDetails(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}