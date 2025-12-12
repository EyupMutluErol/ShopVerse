using Microsoft.EntityFrameworkCore;
using ShopVerse.Business.Abstract;
using ShopVerse.Business.Concrete;
using ShopVerse.DataAccess.Abstract;
using ShopVerse.DataAccess.Concrete.Context;
using ShopVerse.DataAccess.Concrete.EntityFramework;
using ShopVerse.Entities.Concrete;
using ShopVerse.WebUI.Extensions;
using ShopVerse.WebUI.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.ConfigureTurkishValidationMessages();
});

builder.Services.AddDbContext<ShopVerseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); 
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ShopVerseContext>()
.AddErrorDescriber<TrIdentityErrorDescriber>();

builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();

builder.Services.AddScoped<IProductRepository, EfProductRepository>();
builder.Services.AddScoped<IProductService, ProductManager>();

builder.Services.AddScoped<ICartRepository, EfCartRepository>();
builder.Services.AddScoped<ICartService, CartManager>();

builder.Services.AddScoped<ICartItemRepository, EfCartItemRepository>();
builder.Services.AddScoped<ICartItemService, CartItemManager>();

builder.Services.AddScoped<IOrderRepository, EfOrderRepository>();
builder.Services.AddScoped<IOrderService, OrderManager>();

builder.Services.AddScoped<IOrderDetailRepository, EfOrderDetailRepository>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailManager>();

builder.Services.AddScoped<IFavoriteRepository, EfFavoriteRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteManager>();

builder.Services.AddScoped<ImageHelper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
