using Microsoft.AspNetCore.Mvc;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignsController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var values = await _campaignService.GetAllAsync();
            return Ok(values);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var value = await _campaignService.GetByIdAsync(id);
            if (value == null) return NotFound();
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Campaign campaign)
        {
            await _campaignService.AddAsync(campaign);
            return Ok("Kampanya eklendi");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Campaign campaign)
        {
            await _campaignService.UpdateAsync(campaign);
            return Ok("Kampanya güncellendi");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var value = await _campaignService.GetByIdAsync(id);
            if (value == null) return NotFound();
            await _campaignService.DeleteAsync(value);
            return Ok("Kampanya silindi");
        }
    }
}