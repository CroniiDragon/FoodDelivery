using FoodDelivery.Shared.Interfaces;
using FoodDelivery.UserService.DTOs;
using FoodDelivery.UserService.Models;

namespace FoodDelivery.UserService.Interfaces;
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<Customer>> GetAllCustomersAsync();
    Task<IEnumerable<Courier>> GetAllCouriersAsync();
    Task<IEnumerable<Courier>> GetAvailableCouriersAsync();
}

public interface IUserService
{
    Task<UserResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
    Task<UserResponseDto> CreateCourierAsync(CreateCourierDto dto);
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface INotifiable
{
    void SendNotification(string message);
    string GetContactInfo();
}

public interface IAuthenticatable
{
    bool VerifyPassword(string plainPassword);
    string GetRole();
}
