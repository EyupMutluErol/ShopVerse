namespace ShopVerse.Entities.Dtos;

public class ProductFilterDto
{
    public List<int>? CategoryIds { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Search { get; set; }
    public string? SortOrder { get; set; }
}
