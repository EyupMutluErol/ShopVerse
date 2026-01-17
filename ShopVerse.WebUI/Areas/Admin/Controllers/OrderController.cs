using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index(string search, string status, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                orders = orders.Where(x =>
                    x.OrderNumber.ToLower().Contains(search) ||
                    (x.FullName != null && x.FullName.ToLower().Contains(search))
                ).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(typeof(OrderStatus), status, out var statusEnum))
                {
                    orders = orders.Where(x => x.OrderStatus == (OrderStatus)statusEnum).ToList();
                }
            }

            if (startDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                orders = orders.Where(x => x.OrderDate <= end).ToList();
            }

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(orders.OrderByDescending(x => x.OrderDate).ToList());
        }

       
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            
            if (order.OrderStatus == OrderStatus.Delivered)
            {
                if (orderStatus != OrderStatus.Refunded)
                {
                    TempData["Icon"] = "error";
                    TempData["Message"] = "Teslim edilmiş bir sipariş sadece 'İade Edildi' durumuna alınabilir.";
                    return RedirectToAction("Detail", new { area = "Admin", id = orderId });
                }
            }

            if (order.OrderStatus == OrderStatus.Canceled)
            {
                TempData["Icon"] = "error";
                TempData["Message"] = "İptal edilmiş bir siparişin durumu değiştirilemez.";
                return RedirectToAction("Detail", new { area = "Admin", id = orderId });
            }

           
            if (orderStatus == OrderStatus.Delivered && order.OrderStatus != OrderStatus.Delivered)
            {
                order.DeliveryDate = DateTime.Now;
              
            }

            order.OrderStatus = orderStatus;
            await _orderService.UpdateAsync(order);

            TempData["Icon"] = "success";
            TempData["Message"] = $"Sipariş durumu '{orderStatus}' olarak güncellendi.";

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