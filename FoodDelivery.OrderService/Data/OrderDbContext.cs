using FoodDelivery.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order>     Orders     => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(250);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.DeliveryFee).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ItemName).HasMaxLength(150);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Ignore(e => e.TotalPrice); // calculated, not column in DB

            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
