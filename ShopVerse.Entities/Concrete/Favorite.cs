namespace ShopVerse.Entities.Concrete;

public class Favorite:BaseEntity
{
    public string UserId { get; set; }
    public AppUser AppUser { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}
