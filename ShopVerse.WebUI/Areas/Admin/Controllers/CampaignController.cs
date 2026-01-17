using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopVerse.Business.Abstract;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Areas.Admin.Models;
using ShopVerse.WebUI.Utils; 

namespace ShopVerse.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CampaignController : Controller
    {
        private readonly ICampaignService _campaignService;
        private readonly ICategoryService _categoryService;
        private readonly ImageHelper _imageHelper; 

        public CampaignController(
            ICampaignService campaignService,
            ICategoryService categoryService,
            ImageHelper imageHelper) 
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
                string imagePath = "/img/no-image.png"; 

                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadFile(model.ImageFile, "campaigns");
                }

                var campaign = new Campaign
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate.Value,
                    EndDate = model.EndDate.Value,
                    DiscountPercentage = model.DiscountPercentage ?? 0,
                    TargetCategoryId = model.TargetCategoryId,
                    ImageUrl = imagePath,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now,
                    MinProductPrice = model.MinProductPrice,
                    MaxProductPrice = model.MaxProductPrice
                };

                await _campaignService.AddAsync(campaign);

                TempData["AdminSuccess"] = "Kampanya başarıyla oluşturuldu.";
                return RedirectToAction("Index");
            }

            // Hata varsa
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

            var model = new CampaignUpdateViewModel
            {
                Id = campaign.Id,
                Title = campaign.Title,
                Description = campaign.Description,
                StartDate = campaign.StartDate,
                EndDate = campaign.EndDate,
                DiscountPercentage = campaign.DiscountPercentage,
                TargetCategoryId = campaign.TargetCategoryId,
                IsActive = campaign.IsActive,
                CurrentImageUrl = campaign.ImageUrl,
                MinProductPrice = campaign.MinProductPrice,
                MaxProductPrice = campaign.MaxProductPrice
            };

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", campaign.TargetCategoryId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CampaignUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingCampaign = await _campaignService.GetByIdAsync(model.Id);

                if (existingCampaign == null)
                {
                    return NotFound();
                }

                if (model.ImageFile != null)
                {
                    
                    existingCampaign.ImageUrl = await _imageHelper.UploadFile(model.ImageFile, "campaigns");
                }

                existingCampaign.Title = model.Title;
                existingCampaign.Description = model.Description;
                existingCampaign.StartDate = model.StartDate.Value;
                existingCampaign.EndDate = model.EndDate.Value;
                existingCampaign.DiscountPercentage = model.DiscountPercentage ?? 0;
                existingCampaign.TargetCategoryId = model.TargetCategoryId;
                existingCampaign.IsActive = model.IsActive;
                existingCampaign.MinProductPrice = model.MinProductPrice;
                existingCampaign.MaxProductPrice = model.MaxProductPrice;

                await _campaignService.UpdateAsync(existingCampaign);

                TempData["AdminSuccess"] = "Kampanya başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", model.TargetCategoryId);

            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var campaign = await _campaignService.GetByIdAsync(id);
            if (campaign != null)
            {
                if (!string.IsNullOrEmpty(campaign.ImageUrl) && !campaign.ImageUrl.Contains("no-image"))
                {
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