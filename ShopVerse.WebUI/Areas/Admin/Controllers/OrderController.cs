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
            // 1. Tüm Siparişleri Çek
            // Not: İdealde filtreleme Repository katmanında IQueryable ile yapılmalıdır.
            var orders = await _orderService.GetAllAsync();

            // 2. Arama Filtresi (Sipariş No)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                orders = orders.Where(x => x.OrderNumber.ToLower().Contains(search)).ToList();
            }

            // 3. Durum Filtresi
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(typeof(OrderStatus), status, out var statusEnum))
                {
                    orders = orders.Where(x => x.OrderStatus == (OrderStatus)statusEnum).ToList();
                }
            }

            // 4. Tarih Aralığı Filtresi
            if (startDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate <= endDate.Value).ToList();
            }

            // 5. Filtreleri View'da tekrar göstermek için ViewBag'e taşı
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd HH:mm");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd HH:mm");

            return View(orders.OrderByDescending(x => x.OrderDate).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _orderService.GetByIdAsync(orderId);
            if (order != null)
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