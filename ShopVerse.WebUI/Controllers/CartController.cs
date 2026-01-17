using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Models;

namespace ShopVerse.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFavoriteService _favoriteService;

        public CartController(
            IProductService productService,
            ICampaignService campaignService,
            ICouponService couponService,
            UserManager<AppUser> userManager,
            IFavoriteService favoriteService)
        {
            _productService = productService;
            _campaignService = campaignService;
            _couponService = couponService;
            _userManager = userManager;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = GetCartFromSession();
            var appliedCouponCode = HttpContext.Session.GetString("AppliedCoupon");

            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                foreach (var item in cart)
                {
                    item.SalePrice = item.Product.Price;
                }
            }
            else
            {
                
                var activeCampaigns = await _campaignService.GetAllAsync(x => x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);

                foreach (var item in cart)
                {
                    
                    var campaign = activeCampaigns.FirstOrDefault(c =>
                        (c.TargetCategoryId == item.Product.CategoryId || c.TargetCategoryId == null) && 
                        (c.MinProductPrice == null || item.Product.Price >= c.MinProductPrice) &&        
                        (c.MaxProductPrice == null || item.Product.Price <= c.MaxProductPrice)          
                    );

                    decimal itemPrice = item.Product.Price;

                    if (campaign != null)
                    {
                        decimal campaignPrice = item.Product.Price - (item.Product.Price * campaign.DiscountPercentage / 100);
                        itemPrice = campaignPrice;
                    }

                    if (item.Product.DiscountRate > 0)
                    {
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

            decimal totalPrice = cart.Sum(x => x.Quantity * x.SalePrice);
            decimal discountAmount = 0;
            decimal finalPrice = totalPrice;

            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                var coupon = _couponService.GetByCode(appliedCouponCode);

                decimal rawTotal = cart.Sum(x => x.Quantity * x.Product.Price);

                if (coupon != null && coupon.IsActive && coupon.ExpirationDate >= DateTime.Now && rawTotal >= coupon.MinCartAmount)
                {
                    if (coupon.IsPercentage)
                    {
                        discountAmount = totalPrice * (coupon.DiscountAmount / 100);
                    }
                    else
                    {
                        discountAmount = coupon.DiscountAmount;
                    }

                    if (discountAmount > totalPrice) discountAmount = totalPrice;
                    finalPrice = totalPrice - discountAmount;

                    ViewBag.CouponCode = coupon.Code;
                    ViewBag.DiscountAmount = discountAmount;
                }
                else
                {
                    HttpContext.Session.Remove("AppliedCoupon");
                }
            }

            ViewBag.TotalPrice = totalPrice;
            ViewBag.FinalPrice = finalPrice;

            
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var userFavorites = await _favoriteService.GetFavoritesWithProductsAsync(user.Id);
                    ViewBag.FavoriteProductIds = userFavorites.Select(x => x.ProductId).ToList();
                }
            }
            else
            {
                ViewBag.FavoriteProductIds = new List<int>();
            }

            SaveCartToSession(cart);

            return View(new CartViewModel { CartItems = cart });
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                TempData["CouponError"] = "Lütfen bir kod giriniz.";
                return RedirectToAction("Index");
            }

            var coupon = _couponService.GetByCode(code.ToUpper());
            var cart = GetCartFromSession();

            decimal rawTotalPrice = cart.Sum(x => x.Quantity * x.Product.Price);

            if (coupon == null || !coupon.IsActive || coupon.ExpirationDate < DateTime.Now)
            {
                TempData["CouponError"] = "Geçersiz veya süresi dolmuş kupon kodu.";
                return RedirectToAction("Index");
            }

            if (coupon.UserId != null)
            {
                var user = await _userManager.GetUserAsync(User);
                string currentUserId = user != null ? user.Id : null;

                if (string.IsNullOrEmpty(currentUserId) || coupon.UserId != currentUserId)
                {
                    TempData["CouponError"] = "Bu kupon size ait değil.";
                    return RedirectToAction("Index");
                }
            }

            if (rawTotalPrice < coupon.MinCartAmount)
            {
                TempData["CouponError"] = $"Bu kuponu kullanmak için sepet tutarınız en az {coupon.MinCartAmount:C2} olmalıdır.";
                return RedirectToAction("Index");
            }

            

            HttpContext.Session.SetString("AppliedCoupon", coupon.Code);
            TempData["CouponSuccess"] = "Kupon başarıyla uygulandı! (Diğer kampanyalar devre dışı bırakıldı)";

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("AppliedCoupon");
            TempData["CouponSuccess"] = "Kupon kaldırıldı.";
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

            var activeCampaigns = await _campaignService.GetAllAsync(x => x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now);

            var campaign = activeCampaigns.FirstOrDefault(c =>
                (c.TargetCategoryId == product.CategoryId || c.TargetCategoryId == null) &&
                (c.MinProductPrice == null || product.Price >= c.MinProductPrice) &&  
                (c.MaxProductPrice == null || product.Price <= c.MaxProductPrice)     
            );

            decimal campaignPrice = product.Price;
            if (campaign != null)
                campaignPrice = product.Price - (product.Price * campaign.DiscountPercentage / 100);

            decimal productDiscountPrice = product.Price;
            if (product.DiscountRate > 0)
                productDiscountPrice = product.PriceWithDiscount;

            decimal finalPrice = product.Price;

            if (campaign != null && product.DiscountRate > 0)
                finalPrice = Math.Min(campaignPrice, productDiscountPrice);
            else if (campaign != null)
                finalPrice = campaignPrice;
            else if (product.DiscountRate > 0)
                finalPrice = productDiscountPrice;

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(x => x.Product.Id == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.SalePrice = finalPrice; 
            }
            else
            {
                cart.Add(new CartItemModel
                {
                    Product = product,
                    Quantity = quantity,
                    SalePrice = finalPrice
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