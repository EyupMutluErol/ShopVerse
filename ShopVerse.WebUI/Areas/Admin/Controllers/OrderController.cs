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

        // ============================================================
        // SİPARİŞ LİSTESİ (FİLTRELEME İLE)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Index(string search, string status, DateTime? startDate, DateTime? endDate)
        {
            var orders = await _orderService.GetAllAsync();

            // 1. Arama Filtresi (Sipariş No veya Müşteri Adı)
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                orders = orders.Where(x =>
                    x.OrderNumber.ToLower().Contains(search) ||
                    (x.FullName != null && x.FullName.ToLower().Contains(search))
                ).ToList();
            }

            // 2. Durum Filtresi
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse(typeof(OrderStatus), status, out var statusEnum))
                {
                    orders = orders.Where(x => x.OrderStatus == (OrderStatus)statusEnum).ToList();
                }
            }

            // 3. Tarih Aralığı Filtresi
            if (startDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                // Bitiş tarihinin gün sonunu kapsamasını sağla (23:59:59)
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                orders = orders.Where(x => x.OrderDate <= end).ToList();
            }

            // Filtreleri View'da korumak için ViewBag
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentStatus = status;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            // En yeniden eskiye sırala
            return View(orders.OrderByDescending(x => x.OrderDate).ToList());
        }

        // ============================================================
        // DURUM GÜNCELLEME (KRİTİK MANTIK BURADA)
        // ============================================================
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int orderId, OrderStatus orderStatus)
        {
            var order = await _orderService.GetByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // --- KURAL 1: Teslim Edilen Sipariş Sadece 'İade' (Returned) Olabilir ---
            // Eğer sipariş zaten teslim edildiyse, onu tekrar "Kargoda" veya "Bekliyor" yapamayız.
            // Sadece "İade Edildi" (Returned) durumuna geçebilir.
            if (order.OrderStatus == OrderStatus.Delivered)
            {
                if (orderStatus != OrderStatus.Refunded)
                {
                    TempData["Icon"] = "error";
                    TempData["Message"] = "Teslim edilmiş bir sipariş sadece 'İade Edildi' durumuna alınabilir.";
                    return RedirectToAction("Detail", new { area = "Admin", id = orderId });
                }
            }

            // --- KURAL 2: İptal Edilen Sipariş Değiştirilemez ---
            if (order.OrderStatus == OrderStatus.Canceled)
            {
                TempData["Icon"] = "error";
                TempData["Message"] = "İptal edilmiş bir siparişin durumu değiştirilemez.";
                return RedirectToAction("Detail", new { area = "Admin", id = orderId });
            }

            // --- KURAL 3: Teslim Tarihi Kaydı ---
            // Eğer sipariş durumu "Teslim Edildi" (Delivered) olarak değiştiriliyorsa, şu anki zamanı kaydet.
            // Bu, kullanıcının 3 gün iade hakkını hesaplamak için gereklidir.
            if (orderStatus == OrderStatus.Delivered && order.OrderStatus != OrderStatus.Delivered)
            {
                order.DeliveryDate = DateTime.Now;
                // Not: Entity'de 'DeliveryDate' alanı yoksa eklemelisiniz.
                // Eğer yoksa şimdilik bu satırı yorum satırı yapın veya Entity'yi güncelleyin.
            }

            // Durumu güncelle
            order.OrderStatus = orderStatus;
            await _orderService.UpdateAsync(order);

            TempData["Icon"] = "success";
            TempData["Message"] = $"Sipariş durumu '{orderStatus}' olarak güncellendi.";

            return RedirectToAction("Detail", new { area = "Admin", id = orderId });
        }

        // ============================================================
        // DETAY GÖSTERİMİ
        // ============================================================
        public async Task<IActionResult> Detail(int id)
        {
            // Servis katmanında "Include" ile OrderDetails ve Product getirilmeli
            var order = _orderService.GetOrderWithDetails(id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
    }
}