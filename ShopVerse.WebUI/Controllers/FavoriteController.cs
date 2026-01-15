using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShopVerse.WebUI.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly UserManager<AppUser> _userManager;

        public FavoriteController(IFavoriteService favoriteService, UserManager<AppUser> userManager)
        {
            _favoriteService = favoriteService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var favorites = await _favoriteService.GetFavoritesWithProductsAsync(user.Id);

            var model = favorites.Select(x => new ProductCardViewModel
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                Price = x.Product.Price,
                ImageUrl = x.Product.ImageUrl,
                CategoryName = x.Product.Category != null ? x.Product.Category.Name : "Kategorisiz",
                DiscountRate = x.Product.DiscountRate,
                PriceWithDiscount = x.Product.PriceWithDiscount,
                Stock = x.Product.Stock,
                IsHome = x.Product.IsHome
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int productId)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return Json(new { success = false, message = "Lütfen giriş yapınız." });
            }

            await _favoriteService.ToggleFavoriteAsync(user.Id, productId);

            int count = await _favoriteService.GetCountByUserIdAsync(user.Id);

            var favRecord = await _favoriteService.GetByProductAndUserAsync(productId, user.Id);
            bool isFavorited = favRecord != null;

            return Json(new { success = true, count = count, isFavorited = isFavorited });
        }
    }
}