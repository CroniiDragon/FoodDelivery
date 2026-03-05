using FoodDelivery.NotificationService.Data;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.NotificationService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.NotificationService.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;
    public NotificationRepository(NotificationDbContext context) => _context = context;

    public async Task<Notification?> GetByIdAsync(int id)    => await _context.Notifications.FindAsync(id);
    public async Task<IEnumerable<Notification>> GetAllAsync() => await _context.Notifications.ToListAsync();
    public async Task<IEnumerable<Notification>> GetByRecipientAsync(int recipientId)
        => await _context.Notifications.Where(n => n.RecipientId == recipientId).ToListAsync();

    public async Task<Notification> AddAsync(Notification entity)
    {
        await _context.Notifications.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Notification> UpdateAsync(Notification entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var n = await GetByIdAsync(id);
        if (n != null) { _context.Notifications.Remove(n); await _context.SaveChangesAsync(); }
    }

    public async Task<bool> ExistsAsync(int id)
        => await _context.Notifications.AnyAsync(n => n.Id == id);
}
