using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.Entities.Dtos;
using ShopVerse.WebUI.Models;
using System.Diagnostics;

namespace ShopVerse.WebUI.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICampaignService _campaignService;
        private readonly ICouponService _couponService;
        private readonly UserManager<AppUser> _userManager;

        private readonly IFavoriteService _favoriteService;

        public HomeController(
            ILogger<HomeController> logger,
            IProductService productService,
            ICategoryService categoryService,
            ICampaignService campaignService,
            ICouponService couponService,
            UserManager<AppUser> userManager,
            IFavoriteService favoriteService) 
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _campaignService = campaignService;
            _couponService = couponService;
            _userManager = userManager;
            _favoriteService = favoriteService; 
        }

        public async Task<IActionResult> Index(List<int>? categoryIds, decimal? minPrice, decimal? maxPrice, string search, string sortOrder)
        {
            var categories = await _categoryService.GetAllAsync();

            var filterDto = new ProductFilterDto
            {
                CategoryIds = categoryIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Search = search,
                SortOrder = sortOrder
            };

            var products = await _productService.GetFilteredProductsAsync(filterDto);

            

            products = products.Where(x => x.IsActive).ToList();

            
            bool isUserFiltering = (categoryIds != null && categoryIds.Any()) ||
                                   minPrice.HasValue ||
                                   maxPrice.HasValue ||
                                   !string.IsNullOrEmpty(search);

            if (!isUserFiltering)
            {
                products = products.Where(x => x.IsHome).ToList();
            }

            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                  x.IsActive &&
                  x.StartDate <= DateTime.Now &&
                  x.EndDate >= DateTime.Now
            );

            var user = await _userManager.GetUserAsync(User);
            string? currentUserId = user?.Id;

            var activeCoupons = await _couponService.GetAllAsync(x =>
                x.IsActive &&
                x.ExpirationDate >= DateTime.Now &&
                (x.UserId == null || x.UserId == currentUserId)
            );

            ViewBag.ActiveCoupons = activeCoupons;

            if (user != null)
            {
                var userFavorites = await _favoriteService.GetFavoritesWithProductsAsync(user.Id);
                ViewBag.FavoriteProductIds = userFavorites.Select(x => x.ProductId).ToList();
            }
            else
            {
                ViewBag.FavoriteProductIds = new List<int>();
            }

            var model = new HomeViewModel
            {
                FeaturedProducts = products,
                Categories = categories,
                ActiveCampaigns = activeCampaigns.ToList()
            };

            ViewData["CurrentSearch"] = search;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SelectedCategories"] = categoryIds;
            ViewData["SortOrder"] = sortOrder;

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}