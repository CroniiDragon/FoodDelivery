using FoodDelivery.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.UserService.Data;
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Courier> Couriers => Set<Courier>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Config table Customers
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.DeliveryAddress).HasMaxLength(250);
            entity.Property(e => e.City).HasMaxLength(100);
        });

        //Couriers
        modelBuilder.Entity<Courier>(entity =>
        {
            entity.ToTable("Couriers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.VehicleType).HasMaxLength(50);
        });
    }
}
