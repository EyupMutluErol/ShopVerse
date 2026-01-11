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
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.Product.Id == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItemModel
                {
                    Product = product,
                    Quantity = quantity
                });
            }

            SaveCartToSession(cart);

            int cartCount = cart.Sum(x => x.Quantity);

            return Json(new { success = true, message = "Ürün sepete eklendi.", cartCount = cartCount });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var itemToRemove = cart.FirstOrDefault(x => x.Product.Id == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartToSession(cart);

                TempData["Icon"] = "success";
                TempData["Message"] = "Ürün başarıyla silindi.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Increase(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.Product.Id == productId);

            if(item != null)
            {
                item.Quantity++;
                SaveCartToSession(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Decrease(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(x => x.Product.Id == productId);

            if (item != null)
            {
                if(item.Quantity > 1)
                {
                    item.Quantity--;
                    SaveCartToSession(cart);
                }
                else
                {
                    cart.Remove(item);
                    SaveCartToSession(cart);
                    TempData["Icon"] = "success";
                    TempData["Message"] = "Ürün başarıyla silindi.";
                }
            }
            return RedirectToAction("Index");
        }
        private List<CartItemModel> GetCartFromSession()
        {
            return HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
        }

        private void SaveCartToSession(List<CartItemModel> cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }
    }
}