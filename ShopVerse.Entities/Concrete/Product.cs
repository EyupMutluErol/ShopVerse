namespace ShopVerse.Entities.Concrete;

public class Product:BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; }

    // Durumlar
    public bool IsHome { get; set; } // Anasayfada göster
    public bool IsActive { get; set; } // Satışta mı?

    // İlişkiler
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
