using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var values = await _favoriteService.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var values = await _favoriteService.GetFavoritesWithProductsAsync(userId);
            return Ok(values);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Favorite favorite)
        {
            await _favoriteService.AddAsync(favorite);
            return Ok("Favorilere eklendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _favoriteService.GetByIdAsync(id);
            if (value == null) return NotFound();
            await _favoriteService.DeleteAsync(value);
            return Ok("Favorilerden çıkarıldı");
        }
    }
}