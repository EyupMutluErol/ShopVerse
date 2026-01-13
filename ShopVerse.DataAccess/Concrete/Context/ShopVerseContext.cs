using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopVerse.Entities.Concrete;

namespace ShopVerse.DataAccess.Concrete.Context;

public class ShopVerseContext:IdentityDbContext<AppUser,AppRole,string>
{
    public ShopVerseContext(DbContextOptions<ShopVerseContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<Coupon> Coupons { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 


        // Varlıklar arası ilişkiler

        builder.Entity<Product>()
            .HasOne(p=>p.Category)
            .WithMany(c=>c.Products)
            .HasForeignKey(p=>p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o=>o.OrderDetails)
            .HasForeignKey(od=>od.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>()
            .HasOne(o=>o.AppUser)
            .WithMany()
            .HasForeignKey(o=>o.AppUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Cart>()
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Favorite>()
            .HasOne(f=>f.Product)
            .WithMany()
            .HasForeignKey(f=>f.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Favorite>()
            .HasOne(f => f.AppUser)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
