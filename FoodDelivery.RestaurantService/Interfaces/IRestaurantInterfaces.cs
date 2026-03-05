using FoodDelivery.Shared.Interfaces;
using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Models;

namespace FoodDelivery.RestaurantService.Interfaces;

// ISP: Interfete mici si specifice

public interface IRestaurantRepository : IBaseRepository<Restaurant>
{
    Task<IEnumerable<Restaurant>> GetByCityAsync(string city);
    Task<IEnumerable<Restaurant>> GetOpenRestaurantsAsync();
    Task<Restaurant?> GetWithMenuAsync(int restaurantId);
}

public interface IMenuItemRepository : IBaseRepository<MenuItem>
{
    Task<IEnumerable<MenuItem>> GetByRestaurantAsync(int restaurantId);
    Task<IEnumerable<MenuItem>> GetAvailableByRestaurantAsync(int restaurantId);
}

// DIP: Service-ul depinde de aceasta interfata
public interface IRestaurantService
{
    Task<RestaurantResponseDto> CreateAsync(CreateRestaurantDto dto);
    Task<RestaurantResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<RestaurantResponseDto>> GetAllAsync();
    Task<IEnumerable<RestaurantResponseDto>> GetOpenInCityAsync(string city);
    Task<bool> DeleteAsync(int id);
}

public interface IMenuItemService
{
    Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto);
    Task<IEnumerable<MenuItemResponseDto>> GetByRestaurantAsync(int restaurantId);
    Task<bool> ToggleAvailabilityAsync(int menuItemId);
}
