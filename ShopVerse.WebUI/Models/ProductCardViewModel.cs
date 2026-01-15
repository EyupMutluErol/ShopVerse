namespace ShopVerse.WebUI.Models
{
    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }

        public decimal? DiscountRate { get; set; }
        public decimal PriceWithDiscount { get; set; }
        public int Stock { get; set; } 
        public bool IsHome { get; set; } 
    }
}