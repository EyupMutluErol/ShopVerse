using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Enums;


namespace ShopVerse.WebUI.ViewComponents.AdminLayoutViewComponents
{
    public class _AdminNotification : ViewComponent
    {
        private readonly IOrderService _orderService;

        public _AdminNotification(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // _AdminNotification.cs içindeki InvokeAsync metodu:

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allData = await _orderService.GetAllAsync();

            // BURADA DA !IsDeleted FİLTRESİ EKLE
            var pendingCount = allData.Count(x => x.OrderStatus == OrderStatus.Pending && x.IsDeleted == false);

            ViewBag.NotificationCount = pendingCount;
            return View();
        }
    }
}