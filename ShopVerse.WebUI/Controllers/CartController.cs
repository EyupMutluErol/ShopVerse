using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICampaignService _campaignService; // 1. Servis Eklendi

        public CartController(IProductService productService, ICampaignService campaignService)
        {
            _productService = productService;
            _campaignService = campaignService;
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

            // ============================================================
            // FİYAT HESAPLAMA MANTIĞI (En İyi Fiyat Stratejisi)
            // ============================================================

            // 1. Aktif Kampanyaları Çek
            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now
            );

            // 2. Bu ürüne uygun kampanya var mı?
            var campaign = activeCampaigns.FirstOrDefault(c => c.TargetCategoryId == product.CategoryId || c.TargetCategoryId == null);

            // 3. Kampanyalı Fiyat
            decimal campaignPrice = product.Price;
            if (campaign != null)
            {
                campaignPrice = product.Price - (product.Price * campaign.DiscountPercentage / 100);
            }

            // 4. Ürün İndirimli Fiyat
            decimal productDiscountPrice = product.Price;
            if (product.DiscountRate > 0)
            {
                // Eğer entity'de hesaplı geliyorsa: product.PriceWithDiscount
                // Gelmiyorsa manuel hesap: product.Price * (100 - product.DiscountRate) / 100;
                productDiscountPrice = product.PriceWithDiscount;
            }

            // 5. KARŞILAŞTIRMA: Hangisi daha ucuzsa onu seç
            decimal finalPrice = product.Price;

            if (campaign != null && product.DiscountRate > 0)
            {
                // İkisi de varsa en ucuzunu al
                finalPrice = Math.Min(campaignPrice, productDiscountPrice);
            }
            else if (campaign != null)
            {
                finalPrice = campaignPrice;
            }
            else if (product.DiscountRate > 0)
            {
                finalPrice = productDiscountPrice;
            }
            // ============================================================

            // SEPET İŞLEMLERİ
            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.Product.Id == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                // Mevcut ürünün fiyatını da güncel en iyi fiyata çekelim (Kampanya yeni gelmiş olabilir)
                existingItem.SalePrice = finalPrice;
            }
            else
            {
                cart.Add(new CartItemModel
                {
                    Product = product,
                    Quantity = quantity,
                    SalePrice = finalPrice // Hesaplanan fiyatı buraya atıyoruz
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

            if (item != null)
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
                if (item.Quantity > 1)
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