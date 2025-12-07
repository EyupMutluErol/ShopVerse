# ShopVerse - Profesyonel E-Ticaret Projesi

TechStore, .NET 9 kullanýlarak geliþtirilmiþ, gerçek hayat senaryolarýný simüle eden, Çok Katmanlý (N-Layer) mimariye sahip kapsamlý bir E-Ticaret projesidir.

Bu proje; Clean Code prensipleri, SOLID standartlarý ve modern yazýlým geliþtirme pratiklerine uygun olarak, ölçeklenebilir ve kurumsal bir mimariyle tasarlanmýþtýr.

## Mimari Yapý

Proje, sorumluluklarýn ayrýlmasý (Separation of Concerns), sürdürülebilirlik ve test edilebilirlik için **N-Layer Architecture (Çok Katmanlý Mimari)** yapýsýnda kurgulanmýþtýr:

* **ShopVerse.Entities:** Veritabaný tablolarý, varlýklar (Entities) ve temel modeller.
* **ShopVerse.DataAccess:** Veritabaný baðlamý (EF Core Context), migration iþlemleri ve Generic Repository deseninin uygulandýðý katman.
* **ShopVerse.Business:** Ýþ kurallarý, validasyonlar (FluentValidation) ve veri transfer objeleri (DTO).
* **ShopVerse.WebUI:** Müþteriler ve yönetim paneli için geliþtirilen ASP.NET Core MVC arayüzü.
* **ShopVerse.WebAPI:** Dýþ servisler ve farklý platformlar için veri saðlayan RESTful API katmaný.

## Teknolojiler ve Araçlar

* **Framework:** .NET 9.0
* **Veritabaný:** MS SQL Server
* **ORM:** Entity Framework Core (Code First Yaklaþýmý)
* **Kimlik Doðrulama:** ASP.NET Core Identity
* **Doðrulama:** FluentValidation
* **Frontend:** ASP.NET Core MVC, Bootstrap 5, HTML5/CSS3
* **Versiyon Kontrol:** Git

## Temel Özellikler

* **Geliþmiþ Kategori Yönetimi:** Ýç içe sýnýrsýz alt kategori yapýsý (Örn: Elektronik -> Bilgisayar -> Dizüstü -> Gaming).
* **Generic Repository Pattern:** Merkezi ve tekrar kullanýlabilir veri eriþim katmaný.
* **Yönetim Paneli (Admin Dashboard):** Ürün, kategori, sipariþ ve kullanýcý yönetimi için tam kontrol.
* **Sepet ve Sipariþ Süreci:** Oturum bazlý sepet yönetimi ve sipariþ aþamalarý.
* **Ödeme Simülasyonu:** SOLID prensiplerine uygun, geliþtirilebilir sahte banka servisi entegrasyonu.
* **Web API Entegrasyonu:** Verilere dýþarýdan eriþim saðlayan API uç noktalarý.

## Proje Klasör Yapýsý

* ShopVerse.Business
* ShopVerse.DataAccess
* ShopVerse.Entities
* ShopVerse.WebAPI
* ShopVerse.WebUI

##  Lisans ve Telif Hakký

**© 2025 [Eyüp Mutlu Erol] - Tüm Haklarý Saklýdýr.**

Bu proje, sertifikasyon ve eðitim süreci kapsamýnda þahsi olarak geliþtirilmiþtir. Projenin kaynak kodlarýnýn, tasarýmýnýn veya içeriðinin sahibinin yazýlý izni olmaksýzýn kopyalanmasý, çoðaltýlmasý veya ticari amaçla kullanýlmasý yasaktýr.