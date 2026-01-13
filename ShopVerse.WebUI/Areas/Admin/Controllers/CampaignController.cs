using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Utils; // ImageHelper için gerekli namespace

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ICategoryService _categoryService;
        private readonly ImageHelper _imageHelper; // 1. Helper'ı tanımla

        // 2. Constructor'a ImageHelper'ı ekle
        public CampaignController(ICampaignService campaignService, ICategoryService categoryService, ImageHelper imageHelper)
        {
            _campaignService = campaignService;
            _categoryService = categoryService;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            var campaigns = await _campaignService.GetAllAsync();
            return View(campaigns);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Campaign campaign, IFormFile? file)
        {
            // Validasyon temizliği
            ModelState.Remove("ImageUrl");
            ModelState.Remove("TargetCategory"); // İlişki hatasını önlemek için

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    // 3. Helper ile dosya yükleme (Kodlar kısaldı)
                    campaign.ImageUrl = await _imageHelper.UploadFile(file, "campaigns");
                }

                await _campaignService.AddAsync(campaign);
                TempData["Success"] = "Kampanya başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(campaign);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", campaign.TargetCategoryId);
            return View(campaign);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Campaign campaign, IFormFile? file)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("TargetCategory");

            if (ModelState.IsValid)
            {
                var existingCampaign = await _campaignService.GetByIdAsync(campaign.Id);
                if (existingCampaign == null)
                {
                    return NotFound();
                }

                if (file != null)
                {
                    // Yeni dosya varsa Helper ile yükle
                    existingCampaign.ImageUrl = await _imageHelper.UploadFile(file, "campaigns");
                }
                // else: Dosya yoksa eskisini koru (zaten existingCampaign içinde var)

                // Diğer alanları güncelle
                existingCampaign.Title = campaign.Title;
                existingCampaign.Description = campaign.Description;
                existingCampaign.DiscountPercentage = campaign.DiscountPercentage;
                existingCampaign.StartDate = campaign.StartDate;
                existingCampaign.EndDate = campaign.EndDate;
                existingCampaign.IsActive = campaign.IsActive;
                existingCampaign.TargetCategoryId = campaign.TargetCategoryId;

                await _campaignService.UpdateAsync(existingCampaign);

                TempData["Success"] = "Kampanya güncellendi.";
                return RedirectToAction("Index");
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", campaign.TargetCategoryId);
            return View(campaign);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign != null)
            {
                // 4. Silme işlemi için yolu ImageHelper formatına göre ayarla
                if (!string.IsNullOrEmpty(campaign.ImageUrl))
                {
                    // ImageUrl artık "/img/campaigns/dosya.jpg" şeklinde geliyor.
                    // Başındaki "/" işaretini kaldırıp wwwroot ile birleştiriyoruz.
                    var relativePath = campaign.ImageUrl.TrimStart('/');
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                await _campaignService.DeleteAsync(campaign);
                TempData["Success"] = "Kampanya silindi.";
            }

            return RedirectToAction("Index");
        }
    }
}