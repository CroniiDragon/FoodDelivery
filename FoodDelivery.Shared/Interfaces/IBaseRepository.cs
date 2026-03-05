using FoodDelivery.Shared.Models;

namespace FoodDelivery.Shared.Interfaces;

// ISP: Interfata generica separata - nu impunem metode inutile claselor
// DIP: Toate repository-urile depind de aceasta abstractizare
public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
