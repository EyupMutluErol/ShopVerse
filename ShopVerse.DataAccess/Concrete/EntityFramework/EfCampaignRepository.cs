using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.EntityFramework;

public class EfCampaignRepository : EfGenericRepository<Campaign>, ICampaignRepository
{
    public EfCampaignRepository(ShopVerseContext context) : base(context)
    {
    }
}
