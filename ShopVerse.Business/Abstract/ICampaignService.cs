using ShopVerse.Entities.Concrete;

namespace ShopVerse.Business.Abstract;

public interface ICampaignService:IGenericService<Campaign>
{
    Task<Campaign> GetActiveCampaignForCategoryAsync(int categoryId);
}
