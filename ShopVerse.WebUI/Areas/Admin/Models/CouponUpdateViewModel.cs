using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models
{
    public class CouponUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kupon kodu zorunludur.")]
        public string Code { get; set; }

        public decimal? DiscountAmount { get; set; } 
        public bool IsPercentage { get; set; }     
        public decimal? MinCartAmount { get; set; } 

        public DateTime? ExpirationDate { get; set; } 

        public bool IsActive { get; set; }

        public int? CategoryId { get; set; }
        public string? UserId { get; set; }

        public decimal? MinProductPrice { get; set; }
        public decimal? MaxProductPrice { get; set; }
    }
}