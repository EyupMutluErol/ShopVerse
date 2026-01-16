using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Areas.Admin.Models
{
    public class CouponUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kupon kodu zorunludur.")]
        public string Code { get; set; }

        public decimal? DiscountAmount { get; set; } // İndirim Miktarı
        public bool IsPercentage { get; set; }     // Yüzde mi?
        public decimal? MinCartAmount { get; set; } // Minimum Sepet Tutarı

        public DateTime? ExpirationDate { get; set; } // Son Kullanma Tarihi

        public bool IsActive { get; set; }

        // Hedefleme ve Kısıtlamalar
        public int? CategoryId { get; set; }
        public string? UserId { get; set; }

        public decimal? MinProductPrice { get; set; }
        public decimal? MaxProductPrice { get; set; }
    }
}