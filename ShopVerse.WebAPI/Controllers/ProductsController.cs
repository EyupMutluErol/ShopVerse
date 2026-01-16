using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // 1. LİSTELEME (GET)
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // 2. ID'YE GÖRE GETİRME (GET BY ID)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound("Ürün bulunamadı.");
            return Ok(product);
        }

        // 3. EKLEME (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            // ID veritabanında otomatik artar, o yüzden 0 olmalı veya gönderilmemeli.
            await _productService.AddAsync(product);
            return Ok("Ürün başarıyla eklendi.");
        }

        // 4. GÜNCELLEME (PUT) - İyileştirildi
        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            // Güncellenecek ürün var mı kontrol edelim
            var existingProduct = await _productService.GetByIdAsync(product.Id);
            if (existingProduct == null) return NotFound("Güncellenecek ürün bulunamadı.");

            await _productService.UpdateAsync(product);
            return Ok("Ürün güncellendi.");
        }

        // 5. SİLME (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound("Silinecek ürün bulunamadı.");

            await _productService.DeleteAsync(product);
            return Ok("Ürün silindi.");
        }
    }
}