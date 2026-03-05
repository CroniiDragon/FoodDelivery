using FoodDelivery.RestaurantService.Data;
using FoodDelivery.RestaurantService.Interfaces;
using FoodDelivery.RestaurantService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.RestaurantService.Repositories;
public class RestaurantRepository : IRestaurantRepository
{
    private readonly RestaurantDbContext _context;
    public RestaurantRepository(RestaurantDbContext context) => _context = context;

    public async Task<Restaurant?> GetByIdAsync(int id)
        => await _context.Restaurants.FindAsync(id);

    public async Task<Restaurant?> GetWithMenuAsync(int restaurantId)
        => await _context.Restaurants
            .Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == restaurantId);

    public async Task<IEnumerable<Restaurant>> GetAllAsync()
        => await _context.Restaurants.Where(r => !r.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Restaurant>> GetByCityAsync(string city)
        => await _context.Restaurants
            .Where(r => r.City == city && !r.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Restaurant>> GetOpenRestaurantsAsync()
        => await _context.Restaurants
            .Where(r => r.IsOpen && !r.IsDeleted).ToListAsync();

    public async Task<Restaurant> AddAsync(Restaurant entity)
    {
        await _context.Restaurants.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Restaurant> UpdateAsync(Restaurant entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var r = await GetByIdAsync(id);
        if (r == null) return;
        r.IsDeleted  = true;
        r.UpdatedAt  = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Restaurants.AnyAsync(r => r.Id == id);
}

public class MenuItemRepository : IMenuItemRepository
{
    private readonly RestaurantDbContext _context;
    public MenuItemRepository(RestaurantDbContext context) => _context = context;

    public async Task<MenuItem?> GetByIdAsync(int id)
        => await _context.Set<MenuItem>().FindAsync(id);

    public async Task<IEnumerable<MenuItem>> GetAllAsync()
        => await _context.Set<MenuItem>().Where(m => !m.IsDeleted).ToListAsync();

    public async Task<IEnumerable<MenuItem>> GetByRestaurantAsync(int restaurantId)
        => await _context.Set<MenuItem>()
            .Where(m => m.RestaurantId == restaurantId && !m.IsDeleted).ToListAsync();

    public async Task<IEnumerable<MenuItem>> GetAvailableByRestaurantAsync(int restaurantId)
        => await _context.Set<MenuItem>()
            .Where(m => m.RestaurantId == restaurantId && m.IsAvailable && !m.IsDeleted)
            .ToListAsync();

    public async Task<MenuItem> AddAsync(MenuItem entity)
    {
        await _context.Set<MenuItem>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<MenuItem> UpdateAsync(MenuItem entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await GetByIdAsync(id);
        if (item == null) return;
        item.IsDeleted = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Set<MenuItem>().AnyAsync(m => m.Id == id);
}
