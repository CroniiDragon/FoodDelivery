using FoodDelivery.Shared.Interfaces;
using FoodDelivery.UserService.DTOs;
using FoodDelivery.UserService.Models;

namespace FoodDelivery.UserService.Interfaces;

// ISP: Interfete separate pentru operatii diferite

// Interfata pentru repository - acces la date
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<IEnumerable<Courier>> GetAllCouriersAsync();
    Task<IEnumerable<Courier>> GetAvailableCouriersAsync();
}

// Interfata pentru serviciu - logica de business
// DIP: Controller-ul depinde de aceasta abstractizare, nu de implementarea concreta
public interface IUserService
{
    Task<UserResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<UserResponseDto> CreateCourierAsync(CreateCourierDto dto);
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> DeleteAsync(int id);
}

// ISP: Interfata separata doar pentru notificari
public interface INotifiable
{
    void SendNotification(string message);
    string GetContactInfo();
}

// ISP: Interfata separata pentru autentificare
public interface IAuthenticatable
{
    bool VerifyPassword(string plainPassword);
    string GetRole();
}
