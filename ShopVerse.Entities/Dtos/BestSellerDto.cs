namespace ShopVerse.Entities.Dtos;

public class BestSellerDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int SalesCount { get; set; }
}
