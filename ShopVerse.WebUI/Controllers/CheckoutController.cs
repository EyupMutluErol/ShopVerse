using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Enums;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;
        private readonly ICampaignService _campaignService;

        public CheckoutController(
            UserManager<AppUser> userManager,
            IOrderService orderService,
            ICouponService couponService,
            ICampaignService campaignService)
        {
            _userManager = userManager;
            _orderService = orderService;
            _couponService = couponService;
            _campaignService = campaignService;
        }

        // --- Fiyat Hesaplama Yardımcı Metodu (GÜNCELLENDİ) ---
        private async Task<CheckoutViewModel> CalculateOrderTotalsAsync(List<CartItemModel> cart)
        {
            // Session'da Kupon Var mı?
            var appliedCouponCode = HttpContext.Session.GetString("AppliedCoupon");

            // --- MANTIK BAŞLANGICI: CartController ile BİREBİR Aynı Olmalı ---

            // 1. Durum: Kupon VARSA -> Kampanyaları İptal Et, Normal Fiyata Dön
            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                foreach (var item in cart)
                {
                    item.SalePrice = item.Product.Price; // İndirimsiz Fiyat
                }
            }
            // 2. Durum: Kupon YOKSA -> Kampanyaları Uygula
            else
            {
                // Aktif kampanyaları çek
                var activeCampaigns = await _campaignService.GetAllAsync(x => x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);

                foreach (var item in cart)
                {
                    // ========================================================================
                    // GÜNCELLENEN KISIM: Fiyat Aralığı Kontrolü Eklendi
                    // ========================================================================
                    var campaign = activeCampaigns.FirstOrDefault(c =>
                        (c.TargetCategoryId == item.Product.CategoryId || c.TargetCategoryId == null) && // Kategori Tutuyor mu?
                        (c.MinProductPrice == null || item.Product.Price >= c.MinProductPrice) &&        // Min Fiyat Tutuyor mu?
                        (c.MaxProductPrice == null || item.Product.Price <= c.MaxProductPrice)           // Max Fiyat Tutuyor mu?
                    );

                    decimal itemPrice = item.Product.Price;

                    // Kampanya İndirimi
                    if (campaign != null)
                    {
                        itemPrice = item.Product.Price - (item.Product.Price * campaign.DiscountPercentage / 100);
                    }

                    // Ürün Özel İndirimi (DiscountRate)
                    if (item.Product.DiscountRate > 0)
                    {
                        // Hem kampanya hem ürün indirimi varsa en uygun olanı seç
                        if (campaign != null)
                        {
                            itemPrice = Math.Min(itemPrice, item.Product.PriceWithDiscount);
                        }
                        else
                        {
                            itemPrice = item.Product.PriceWithDiscount;
                        }
                    }

                    item.SalePrice = itemPrice;
                }
            }
            // --- MANTIK BİTİŞİ ---

            // Hesaplamalar
            decimal subTotal = cart.Sum(x => x.Quantity * x.SalePrice);
            decimal discountAmount = 0;
            decimal shippingPrice = (subTotal >= 500) ? 0 : 39.90m;

            // Kupon İndirimini Hesapla (Eğer kupon varsa)
            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                var coupon = _couponService.GetByCode(appliedCouponCode);

                // Kupon için de indirimsiz (ham) sepet tutarını kontrol edelim
                decimal rawTotal = cart.Sum(x => x.Quantity * x.Product.Price);

                // Kupon geçerlilik kontrolü
                if (coupon != null && coupon.IsActive && coupon.ExpirationDate >= DateTime.Now && rawTotal >= coupon.MinCartAmount)
                {
                    if (coupon.IsPercentage)
                    {
                        discountAmount = subTotal * (coupon.DiscountAmount / 100);
                    }
                    else
                    {
                        discountAmount = coupon.DiscountAmount;
                    }

                    if (discountAmount > subTotal) discountAmount = subTotal;
                }
                else
                {
                    // Şartlar sağlanmıyorsa kuponu sessizce düşür (Güvenlik)
                    HttpContext.Session.Remove("AppliedCoupon");
                    // Not: Bu durumda sonraki istekte fiyatlar tekrar hesaplanacaktır.
                }
            }

            decimal grandTotal = (subTotal - discountAmount) + shippingPrice;

            return new CheckoutViewModel
            {
                CartItems = cart,
                SubTotal = subTotal,
                DiscountAmount = discountAmount,
                ShippingTotal = shippingPrice,
                GrandTotal = grandTotal
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            if (cart.Count == 0)
            {
                TempData["UserType"] = "error";
                TempData["UserMessage"] = "Sepetiniz boş, sipariş veremezsiniz";
                return RedirectToAction("Index", "Cart");
            }

            // 1. Hesaplamaları Yap (Async çağrı)
            var model = await CalculateOrderTotalsAsync(cart);

            // 2. Kullanıcı Bilgilerini Doldur
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.User = user;
                model.FullName = user.Name + " " + user.Surname;
                model.PhoneNumber = user.PhoneNumber;
                model.City = user.City;
                // Adres bilgileri IdentityUser'da yoksa boş gelebilir, kullanıcı doldurmalı
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(CheckoutViewModel inputModel)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            // GÜVENLİK: Fiyatları sunucu tarafında tekrar hesapla! (Manipülasyonu önlemek için)
            var calculatedModel = await CalculateOrderTotalsAsync(cart);

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);

                var order = new Order
                {
                    OrderNumber = "SP-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    AppUserId = user.Id,

                    // Formdan gelen adres bilgileri
                    FullName = inputModel.FullName,
                    AddressLine = inputModel.AddressLine,
                    City = inputModel.City,
                    District = inputModel.District,
                    PhoneNumber = inputModel.PhoneNumber,

                    // ÖNEMLİ: Veritabanına sunucuda hesaplanan GÜVENLİ tutarı yazıyoruz
                    TotalPrice = calculatedModel.GrandTotal,

                    OrderDetails = new List<OrderDetail>()
                };

                foreach (var item in cart)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = item.Product.Id,
                        // Veritabanına o anki geçerli (indirimli veya normal) birim fiyatı yaz
                        Price = item.SalePrice,
                        Quantity = item.Quantity
                    });
                }

                try
                {
                    // --- ÖDEME ENTEGRASYONU BURADA YAPILIR ---
                    // calculatedModel.GrandTotal tutarı çekilir.

                    await _orderService.AddAsync(order);

                    // Başarılıysa Temizlik
                    HttpContext.Session.Remove("Cart");
                    HttpContext.Session.Remove("AppliedCoupon");

                    // Sipariş Başarılı Mesajı
                    TempData["OrderSuccess"] = "Siparişiniz başarıyla oluşturuldu! Sipariş numaranız: " + order.OrderNumber;

                    return RedirectToAction("Index", "Profile");
                }
                catch (Exception ex)
                {
                    TempData["UserType"] = "error";
                    TempData["UserMessage"] = "Sipariş oluşturulurken bir hata oluştu: " + ex.Message;

                    // Hata durumunda modeli tekrar doldur
                    inputModel.CartItems = calculatedModel.CartItems;
                    inputModel.SubTotal = calculatedModel.SubTotal;
                    inputModel.DiscountAmount = calculatedModel.DiscountAmount;
                    inputModel.ShippingTotal = calculatedModel.ShippingTotal;
                    inputModel.GrandTotal = calculatedModel.GrandTotal;

                    return View("Index", inputModel);
                }
            }

            // Validasyon hatası varsa verileri geri yükle
            inputModel.CartItems = calculatedModel.CartItems;
            inputModel.SubTotal = calculatedModel.SubTotal;
            inputModel.DiscountAmount = calculatedModel.DiscountAmount;
            inputModel.ShippingTotal = calculatedModel.ShippingTotal;
            inputModel.GrandTotal = calculatedModel.GrandTotal;

            return View("Index", inputModel);
        }
    }
}