using System.ComponentModel.DataAnnotations.Schema;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.Entities.Concrete;

public class Coupon : BaseEntity
{
    public string Code { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } 

    public bool IsPercentage { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal MinCartAmount { get; set; } 

    public DateTime ExpirationDate { get; set; } 
    public bool IsActive { get; set; }

   
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinProductPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxProductPrice { get; set; } 

    public int? CategoryId { get; set; }
    public Category? Category { get; set; } 

    public string? UserId { get; set; }
    public AppUser? AppUser { get; set; }
}