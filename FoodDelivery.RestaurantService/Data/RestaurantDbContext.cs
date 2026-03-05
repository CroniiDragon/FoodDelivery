using FoodDelivery.RestaurantService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.RestaurantService.Data;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<FoodItem>   FoodItems   => Set<FoodItem>();
    public DbSet<DrinkItem>  DrinkItems  => Set<DrinkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPH (Table Per Hierarchy) - FoodItem si DrinkItem in acelasi tabel MenuItems
        // EF Core foloseste o coloana discriminator pentru a sti ce tip este
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("MenuItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);
            entity.Property(e => e.Category).HasMaxLength(50);

            entity.HasOne(e => e.Restaurant)
                  .WithMany(r => r.MenuItems)
                  .HasForeignKey(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Coloana discriminator pentru TPH
            entity.HasDiscriminator<string>("ItemType")
                  .HasValue<FoodItem>("Food")
                  .HasValue<DrinkItem>("Drink");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("Restaurants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Cuisine).HasMaxLength(100);
        });
    }
}
