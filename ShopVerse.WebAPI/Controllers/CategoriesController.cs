using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var values = await _categoryService.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _categoryService.GetByIdAsync(id);
            if (value == null) return NotFound();
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            await _categoryService.AddAsync(category);
            return Ok("Kategori başarıyla eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Category category)
        {
            await _categoryService.UpdateAsync(category);
            return Ok("Kategori güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _categoryService.GetByIdAsync(id);
            if (value == null) return NotFound();
            await _categoryService.DeleteAsync(value);
            return Ok("Kategori silindi");
        }
    }
}