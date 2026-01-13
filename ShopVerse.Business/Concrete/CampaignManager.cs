using ShopVerse.Business.Abstract;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Concrete
{
    public class CampaignManager:GenericManager<Campaign>, ICampaignService
    {
        private readonly ICampaignRepository _campaignRepository;
        public CampaignManager(ICampaignRepository campaignRepository) : base(campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        public async Task<Campaign> GetActiveCampaignForCategoryAsync(int categoryId)
        {
            var campaigns = await _campaignRepository.GetAllAsync(x =>
               x.IsActive &&
               x.TargetCategoryId == categoryId &&
               x.StartDate <= DateTime.Now &&
               x.EndDate >= DateTime.Now
           );

            return campaigns.FirstOrDefault();
        }
    }
}
