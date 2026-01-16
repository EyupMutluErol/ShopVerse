using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopVerse.WebUI.Models
{
    public class CheckoutViewModel
    {
        // --- GİRİŞ ALANLARI (Input) ---

        [Required(ErrorMessage = "Ad Soyad zorunludur")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Adres zorunludur")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur")]
        public string City { get; set; }

        [Required(ErrorMessage = "İlçe zorunludur")]
        public string District { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Kart üzerindeki isim zorunludur")]
        public string CardName { get; set; }

        [Required(ErrorMessage = "Kart numarası zorunludur")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Ay zorunludur")]
        public string ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Yıl zorunludur")]
        public string ExpirationYear { get; set; }

        [Required(ErrorMessage = "CVV zorunludur")]
        public string Cvv { get; set; }

        // --- GÖSTERİM ALANLARI (Output) ---

        public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        [ValidateNever]
        public AppUser User { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}