using FoodDelivery.UserService.Data;
using FoodDelivery.UserService.Interfaces;
using FoodDelivery.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.UserService.Repositories;

// DIP: Implementarea concreta a IUserRepository
// SRP: Responsabilitate unica - acces la date pentru utilizatori
public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    // DIP: Injectam contextul prin constructor
    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        // Cautam in ambele tabele
        User? user = await _context.Customers.FindAsync(id);
        return user ?? await _context.Couriers.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        var couriers  = await _context.Couriers.ToListAsync();
        return customers.Cast<User>().Concat(couriers.Cast<User>());
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        User? user = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        return user ?? await _context.Couriers.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        => await _context.Customers.Where(c => !c.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Courier>> GetAllCouriersAsync()
        => await _context.Couriers.Where(c => !c.IsDeleted).ToListAsync();

    public async Task<IEnumerable<Courier>> GetAvailableCouriersAsync()
        => await _context.Couriers.Where(c => c.IsAvailable && !c.IsDeleted).ToListAsync();

    public async Task<User> AddAsync(User entity)
    {
        if (entity is Customer customer)
            await _context.Customers.AddAsync(customer);
        else if (entity is Courier courier)
            await _context.Couriers.AddAsync(courier);

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<User> UpdateAsync(User entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            // Soft delete - nu stergem din baza de date
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Customers.AnyAsync(c => c.Id == id)
            || await _context.Couriers.AnyAsync(c => c.Id == id);
    }
}
