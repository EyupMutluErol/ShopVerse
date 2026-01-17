using Microsoft.AspNetCore.Mvc;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            var count = cart?.Sum(x => x.Quantity) ?? 0;

            return View(count);
        }
    }
}