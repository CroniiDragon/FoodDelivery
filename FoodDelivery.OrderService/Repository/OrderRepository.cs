using FoodDelivery.OrderService.Data;
using FoodDelivery.OrderService.Interfaces;
using FoodDelivery.OrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.OrderService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    public OrderRepository(OrderDbContext context) => _context = context;

    public async Task<Order?> GetByIdAsync(int id)     => await _context.Orders.FindAsync(id);
    public async Task<IEnumerable<Order>> GetAllAsync() => await _context.Orders.Include(o => o.Items).ToListAsync();

    public async Task<Order?> GetWithItemsAsync(int orderId)
        => await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        => await _context.Orders.Include(o => o.Items).Where(o => o.CustomerId == customerId).ToListAsync();

    public async Task<IEnumerable<Order>> GetByRestaurantAsync(int restaurantId)
        => await _context.Orders.Include(o => o.Items).Where(o => o.RestaurantId == restaurantId).ToListAsync();

    public async Task<Order> AddAsync(Order entity)
    {
        await _context.Orders.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Order> UpdateAsync(Order entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var o = await GetByIdAsync(id);
        if (o != null) { o.IsDeleted = true; await _context.SaveChangesAsync(); }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Orders.AnyAsync(o => o.Id == id);
}
