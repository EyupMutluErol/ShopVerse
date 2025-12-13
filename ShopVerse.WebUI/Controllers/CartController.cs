using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;

        public CartController(IProductService productService)
        {
            _productService = productService;
        }
        public ActionResult Index()
        {
            var cart = GetCartFromSession();

            var cartViewModel = new CartViewModel
            {
                CartItems = cart
            };
            return View(cartViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId,int quantity = 1)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var cart = GetCartFromSession();

            var existingItem = cart.FirstOrDefault(x=>x.Product.Id == productId);
            if(existingItem != null)
            {
                existingItem.Quantity++; // ürün varsa sayısını arttır
            } 
            else
            {
                cart.Add(new CartItemModel // ürün yoksa ekle
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            SaveCartToSession(cart);

            TempData["Icon"] = "success";
            TempData["Message"] = "Ürün sepete başarıyla eklendi";

            return RedirectToAction("Index","Home");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var itemToRemove = cart.FirstOrDefault(x=>x.Product.Id == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartToSession(cart);

                TempData["Icon"] = "warning"; 
                TempData["Message"] = "Ürün sepetten silindi";
            }
            return RedirectToAction("Index");
        }

        // Yardımcı Metot: Session'dan Sepeti Getir
        private List<CartItemModel> GetCartFromSession()
        {
            // Eğer "Cart" anahtarı boşsa yeni bir liste oluştur
            return HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        }
        // Yardımcı Metot: Sepeti Session'a Kaydet
        private void SaveCartToSession(List<CartItemModel> cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }
    }
}
