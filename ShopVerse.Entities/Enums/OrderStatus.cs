namespace ShopVerse.Entities.Enums;

public enum OrderStatus
{
    Pending = 0,        // Sipariş alındı, onay bekliyor
    Approved = 1,       // Onaylandı, hazırlanıyor
    Shipped = 2,        // Kargoya verildi
    Delivered = 3,      // Teslim edildi
    Canceled = 4,       // İptal edildi
    Refunded = 5        // İade edildi
}
