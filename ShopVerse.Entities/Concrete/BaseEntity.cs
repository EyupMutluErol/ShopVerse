using ShopVerse.Entities.Abstract;

namespace ShopVerse.Entities.Concrete;

public class BaseEntity:IEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; } = false;
}
