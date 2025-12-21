using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Enums;


namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles= "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders.OrderByDescending(x => x.OrderDate).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if(order != null)
            {
                order.OrderStatus = orderStatus;
                await _orderService.UpdateAsync(order);
            }
            return RedirectToAction("Detail", new { area = "Admin", id = orderId });
        }

        public async Task<IActionResult> Detail(int id)
        {
            var order = _orderService.GetOrderWithDetails(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}
