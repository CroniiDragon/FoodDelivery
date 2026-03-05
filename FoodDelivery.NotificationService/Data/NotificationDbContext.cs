using FoodDelivery.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.NotificationService.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Channel).HasMaxLength(20);
            entity.Property(e => e.RecipientType).HasMaxLength(20);
        });
    }
}
