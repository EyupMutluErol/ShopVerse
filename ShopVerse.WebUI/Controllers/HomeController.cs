using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Dtos;
using ShopVerse.WebUI.Models;
using System.Diagnostics;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICampaignService _campaignService;


        public HomeController(ILogger<HomeController> logger,IProductService productService, ICategoryService categoryService, ICampaignService campaignService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
            _campaignService = campaignService;
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

            var activeCampaigns = await _campaignService.GetAllAsync(x =>
                    x.IsActive &&
                    x.StartDate <= DateTime.Now &&
                    x.EndDate >= DateTime.Now
            );

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
