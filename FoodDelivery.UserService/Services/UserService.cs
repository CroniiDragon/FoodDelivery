using FoodDelivery.UserService.DTOs;
using FoodDelivery.UserService.Interfaces;
using FoodDelivery.UserService.Models;

namespace FoodDelivery.UserService.Services;
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name            = dto.Name,
            Email           = dto.Email,
            Phone           = dto.Phone,
            DeliveryAddress = dto.DeliveryAddress,
            City            = dto.City,
        };
        customer.SetPassword(dto.Password);

        var saved = await _repository.AddAsync(customer);
        return MapToDto(saved);
    }

    public async Task<UserResponseDto> CreateCourierAsync(CreateCourierDto dto)
    {
        var courier = new Courier
        {
            Name        = dto.Name,
            Email       = dto.Email,
            Phone       = dto.Phone,
            VehicleType = dto.VehicleType,
        };
        courier.SetPassword(dto.Password);

        var saved = await _repository.AddAsync(courier);
        return MapToDto(saved);
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);
        if (user == null || !user.VerifyPassword(dto.Password) || !user.IsActive)
            return null;
        return MapToDto(user);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _repository.ExistsAsync(id)) return false;
        await _repository.DeleteAsync(id);
        return true;
    }

    private static UserResponseDto MapToDto(Models.User user) => new()
    {
        Id        = user.Id,
        Name      = user.Name,
        Email     = user.Email,
        Phone     = user.Phone,
        Role      = user.GetRole(),
        IsActive  = user.IsActive,
        CreatedAt = user.CreatedAt,
    };
}
