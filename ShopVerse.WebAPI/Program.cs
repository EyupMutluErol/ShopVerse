using Microsoft.EntityFrameworkCore;
using ShopVerse.Business.Abstract;
using ShopVerse.Business.Concrete;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.DataAccess.Concrete.EntityFramework;
using ShopVerse.Entities.Concrete;
using Microsoft.OpenApi.Models; // <-- BU SATIR OpenApiInfo HATASINI ÇÖZER

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<ShopVerseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity Kurulumu
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ShopVerseContext>();

// 3. Dependency Injection (Tüm Servisler)
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<IProductRepository, EfProductRepository>();

builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();

builder.Services.AddScoped<IOrderService, OrderManager>();
builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();

builder.Services.AddScoped<ICampaignService, CampaignManager>();
builder.Services.AddScoped<ICampaignRepository, EfCampaignRepository>();

builder.Services.AddScoped<ICouponService, CouponManager>();
builder.Services.AddScoped<ICouponRepository, EfCouponRepository>();

// 4. API Ayarlarý
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShopVerse API", Version = "v1" });
});

var app = builder.Build();

// 5. Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopVerse API V1");
        c.RoutePrefix = string.Empty; // Ana sayfada Swagger açýlsýn
    });
}

app.UseHttpsRedirection();
app.UseAuthentication(); // Identity için gerekli
app.UseAuthorization();

// Try-Catch bloðunu kaldýrdýk, .csproj düzenlemesi sorunu kökten çözdüðü için gerek kalmadý.
app.MapControllers();

app.Run();