using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;

namespace ShopVerse.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;

        // YENİ EKLENEN SERVİSLER
        private readonly IFavoriteService _favoriteService;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(
            IProductService productService,
            ICategoryService categoryService,
            ICampaignService campaignService,
            ICouponService couponService,
            IFavoriteService favoriteService,
            UserManager<AppUser> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _campaignService = campaignService;
            _couponService = couponService;
            _favoriteService = favoriteService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(List<int>? categoryIds, decimal? minPrice, decimal? maxPrice, string search, string sortOrder, bool showDeals = false)
        {
            // 1. Kategorileri Çek
            ViewBag.Categories = await _categoryService.GetAllAsync();

            // 2. Aktif Kampanyaları Çek
            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                x.IsActive &&
                x.StartDate <= DateTime.Now &&
                x.EndDate >= DateTime.Now
            );
            ViewBag.ActiveCampaigns = activeCampaigns;

            // 3. Aktif Kuponlar
            var activeCoupons = await _couponService.GetAllAsync(x => x.IsActive && x.ExpirationDate >= DateTime.Now);
            ViewBag.ActiveCoupons = activeCoupons;

            // 4. Filtreleme İşlemleri
            var filterDto = new ProductFilterDto
            {
                CategoryIds = categoryIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Search = search,
                SortOrder = sortOrder
            };

            var products = await _productService.GetFilteredProductsAsync(filterDto);

            // ========================================================================
            // 5. GÜNCELLENEN MANTIK (İŞLEVSELLİK DÜZELTİLDİ)
            // ========================================================================

            // KURAL: Mağaza (Tüm Ürünler) sayfasında bir ürünün görünmesi için 
            // SADECE 'IsActive (Satışta)' olması yeterlidir. 
            // IsHome kapalı olsa bile burada listelenmelidir.
            products = products.Where(p => p.IsActive).ToList();

            // Not: Buradaki 'if (!hasAnyFilter)' bloğunu kaldırdık. 
            // Çünkü burası anasayfa değil, tüm ürünlerin listelendiği mağaza sayfası.
            // ========================================================================

            // 6. FIRSAT FİLTRESİ (SHOW DEALS)
            if (showDeals)
            {
                products = products.Where(p =>
                    p.DiscountRate > 0 ||
                    activeCampaigns.Any(c => c.TargetCategoryId == p.CategoryId || c.TargetCategoryId == null)
                ).ToList();

                ViewData["IsDealsPage"] = true;
            }

            // 7. Filtrelerin Ekranda Korunması İçin ViewData
            ViewData["CurrentSearch"] = search;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SelectedCategories"] = categoryIds;
            ViewData["SortOrder"] = sortOrder;
            ViewData["ShowDeals"] = showDeals;

            // 8. FAVORİLERİ GETİR
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

            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // 1. Tüm Aktif Kampanyaları Çek
            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                x.IsActive && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now
            );

            // 2. Bu ürüne uygun kampanya var mı?
            var campaign = activeCampaigns.FirstOrDefault(c => c.TargetCategoryId == product.CategoryId || c.TargetCategoryId == null);

            // 3. Fiyat Hesaplamaları
            decimal campaignPrice = product.Price;
            if (campaign != null)
            {
                campaignPrice = product.Price - (product.Price * campaign.DiscountPercentage / 100);
            }

            decimal productDiscountPrice = product.Price;
            if (product.DiscountRate > 0)
            {
                productDiscountPrice = product.PriceWithDiscount;
            }

            // 4. Karşılaştırma (En Düşük Fiyat Kazanır)
            decimal finalPrice = product.Price;
            string discountType = "None"; // None, Campaign, ProductDiscount
            int discountRate = 0;

            if (campaign != null && product.DiscountRate > 0)
            {
                if (campaignPrice < productDiscountPrice)
                {
                    finalPrice = campaignPrice;
                    discountType = "Campaign";
                    discountRate = campaign.DiscountPercentage;
                }
                else
                {
                    finalPrice = productDiscountPrice;
                    discountType = "ProductDiscount";
                    discountRate = (int)product.DiscountRate;
                }
            }
            else if (campaign != null)
            {
                finalPrice = campaignPrice;
                discountType = "Campaign";
                discountRate = campaign.DiscountPercentage;
            }
            else if (product.DiscountRate > 0)
            {
                finalPrice = productDiscountPrice;
                discountType = "ProductDiscount";
                discountRate = (int)product.DiscountRate;
            }

            // 5. View'a Veri Taşıma
            ViewBag.FinalPrice = finalPrice;
            ViewBag.DiscountType = discountType;
            ViewBag.DiscountRate = discountRate;

            // ============================================================
            // 6. FAVORİ KONTROLÜ
            // ============================================================
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

            return View(product);
        }
    }
}