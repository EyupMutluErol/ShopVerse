using Microsoft.AspNetCore.Mvc;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        // Session'a erişmek için constructor'a gerek yok, HttpContext yeterli.
        public IViewComponentResult Invoke()
        {
            // Session'dan sepet listesini çek
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

            // Eğer sepet varsa adetleri topla, yoksa 0 dön
            var count = cart?.Sum(x => x.Quantity) ?? 0;

            return View(count);
        }
    }
}