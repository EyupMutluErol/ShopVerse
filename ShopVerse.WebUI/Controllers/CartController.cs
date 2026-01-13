using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;
using Microsoft.AspNetCore.Http; // Session işlemleri için
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ShopVerse.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;

        // Constructor'a ICouponService eklendi
        public CartController(IProductService productService, ICampaignService campaignService, ICouponService couponService)
        {
            _productService = productService;
            _campaignService = campaignService;
            _couponService = couponService;
        }

        public ActionResult Index()
        {
            var cart = GetCartFromSession();

            // 1. Sepet Toplamını Hesapla (SalePrice üzerinden)
            decimal totalPrice = cart.Sum(x => x.Quantity * x.SalePrice);
            decimal discountAmount = 0;
            decimal finalPrice = totalPrice;

            // 2. KUPON KONTROLÜ: Session'da kayıtlı kupon var mı?
            var appliedCouponCode = HttpContext.Session.GetString("AppliedCoupon");

            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                var coupon = _couponService.GetByCode(appliedCouponCode);

                // Kupon hala geçerli mi ve sepet tutarı minimum limiti karşılıyor mu?
                if (coupon != null && coupon.IsActive && coupon.ExpirationDate >= DateTime.Now && totalPrice >= coupon.MinCartAmount)
                {
                    if (coupon.IsPercentage)
                    {
                        // Yüzde İndirimi
                        discountAmount = totalPrice * (coupon.DiscountAmount / 100);
                    }
                    else
                    {
                        // Tutar İndirimi
                        discountAmount = coupon.DiscountAmount;
                    }

                    // İndirim sepet tutarından büyükse, sepet 0 TL olsun (Eksiye düşmesin)
                    if (discountAmount > totalPrice) discountAmount = totalPrice;

                    finalPrice = totalPrice - discountAmount;

                    // View'a gönderilecek veriler
                    ViewBag.CouponCode = coupon.Code;
                    ViewBag.DiscountAmount = discountAmount;
                }
                else
                {
                    // Şartlar artık sağlanmıyorsa (örn: ürün çıkarıldı ve limit altına düştü) kuponu düşür
                    HttpContext.Session.Remove("AppliedCoupon");
                    // İsteğe bağlı: Kullanıcıya bilgi verilebilir
                    // TempData["Error"] = "Sepet tutarı değiştiği için kupon kaldırıldı.";
                }
            }

            // Fiyatları View'a taşı
            ViewBag.TotalPrice = totalPrice;
            ViewBag.FinalPrice = finalPrice;

            var cartViewModel = new CartViewModel
            {
                CartItems = cart
            };
            return View(cartViewModel);
        }

        // --- YENİ EKLENEN: KUPON UYGULAMA METODU ---
        [HttpPost]
        public IActionResult ApplyCoupon(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                TempData["Error"] = "Lütfen bir kod giriniz.";
                return RedirectToAction("Index");
            }

            var coupon = _couponService.GetByCode(code.ToUpper());
            var cart = GetCartFromSession();
            decimal totalPrice = cart.Sum(x => x.Quantity * x.SalePrice);

            // 1. Kupon var mı, aktif mi, süresi dolmuş mu?
            if (coupon == null || !coupon.IsActive || coupon.ExpirationDate < DateTime.Now)
            {
                TempData["Error"] = "Geçersiz veya süresi dolmuş kupon kodu.";
                return RedirectToAction("Index");
            }

            // 2. Minimum sepet tutarı kontrolü
            if (totalPrice < coupon.MinCartAmount)
            {
                TempData["Error"] = $"Bu kuponu kullanmak için sepet tutarınız en az {coupon.MinCartAmount:C2} olmalıdır.";
                return RedirectToAction("Index");
            }

            // 3. Başarılıysa Session'a kaydet
            HttpContext.Session.SetString("AppliedCoupon", coupon.Code);
            TempData["Success"] = "Kupon başarıyla uygulandı!";

            return RedirectToAction("Index");
        }

        // --- YENİ EKLENEN: KUPON KALDIRMA METODU ---
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCoupon");
            TempData["Success"] = "Kupon kaldırıldı.";
            return RedirectToAction("Index");
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
            // Ürün silinince sepet tutarı değişeceği için kuponu tekrar kontrol etmek gerekebilir.
            // Index metodu her açılışta bunu kontrol ettiği için sorun olmaz.
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