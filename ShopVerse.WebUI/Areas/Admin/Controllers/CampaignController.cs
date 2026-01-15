using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using ShopVerse.WebUI.Utils; // ImageHelper için namespace

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ICategoryService _categoryService;
        private readonly ImageHelper _imageHelper; // Helper Tanımlandı

        public CampaignController(
            ICampaignService campaignService,
            ICategoryService categoryService,
            ImageHelper imageHelper) // Constructor Injection
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
        public async Task<IActionResult> Create(CampaignAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. RESİM YÜKLEME (Helper Kullanarak)
                string imagePath = "/img/no-image.png"; // Varsayılan görsel

                if (model.ImageFile != null)
                {
                    // "campaigns" klasörüne yükle. Helper, klasör yoksa oluşturur.
                    imagePath = await _imageHelper.UploadFile(model.ImageFile, "campaigns");
                }

                // 2. MAPPING (ViewModel -> Entity)
                var campaign = new Campaign
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate.Value,
                    EndDate = model.EndDate.Value,
                    DiscountPercentage = model.DiscountPercentage ?? 0,
                    TargetCategoryId = model.TargetCategoryId, // Null olabilir

                    ImageUrl = imagePath, // Helper'dan dönen yolu kaydet

                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };

                // 3. KAYIT
                await _campaignService.AddAsync(campaign);

                // 4. BAŞARI MESAJI (Admin Özel Key)
                TempData["AdminSuccess"] = "Kampanya başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            // Validasyon Hatası Varsa Dropdown'ı Tekrar Doldur
            ViewBag.Categories = new SelectList(await _categoryService.GetAllAsync(), "Id", "Name");
            return View(model);
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
            // Dropdown'da mevcut kategoriyi seçili getir
            ViewBag.Categories = new SelectList(categories, "Id", "Name", campaign.TargetCategoryId);

            return View(campaign);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Campaign campaign, IFormFile? file)
        {
            // Validasyondan ImageUrl ve Navigation Property'yi çıkar
            ModelState.Remove("ImageUrl");
            ModelState.Remove("TargetCategory");

            if (ModelState.IsValid)
            {
                var existingCampaign = await _campaignService.GetByIdAsync(campaign.Id);
                if (existingCampaign == null)
                {
                    return NotFound();
                }

                // YENİ RESİM VARSA YÜKLE
                if (file != null)
                {
                    // Eski resmi sil (Opsiyonel ama iyi olur)
                    // if (existingCampaign.ImageUrl != "/img/no-image.png") { ... silme kodu ... }

                    // Yeni resmi yükle
                    existingCampaign.ImageUrl = await _imageHelper.UploadFile(file, "campaigns");
                }
                // else: Dosya yoksa mevcut ImageUrl korunur

                // DİĞER ALANLARI GÜNCELLE
                existingCampaign.Title = campaign.Title;
                existingCampaign.Description = campaign.Description;
                existingCampaign.DiscountPercentage = campaign.DiscountPercentage;
                existingCampaign.StartDate = campaign.StartDate;
                existingCampaign.EndDate = campaign.EndDate;
                existingCampaign.IsActive = campaign.IsActive;
                existingCampaign.TargetCategoryId = campaign.TargetCategoryId;

                await _campaignService.UpdateAsync(existingCampaign);

                TempData["AdminSuccess"] = "Kampanya başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            // Hata varsa view'ı doldur
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", campaign.TargetCategoryId);

            return View(campaign);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign != null)
            {
                // RESİM SİLME İŞLEMİ (Opsiyonel: Varsayılan resmi silme)
                if (!string.IsNullOrEmpty(campaign.ImageUrl) && !campaign.ImageUrl.Contains("no-image"))
                {
                    // ImageUrl: "/img/campaigns/resim.jpg"
                    // Path: "wwwroot/img/campaigns/resim.jpg"
                    var relativePath = campaign.ImageUrl.TrimStart('/');
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }

                await _campaignService.DeleteAsync(campaign);
                TempData["AdminSuccess"] = "Kampanya başarıyla silindi.";
            }
            else
            {
                TempData["AdminError"] = "Kampanya bulunamadı.";
            }

            return RedirectToAction("Index");
        }
    }
}