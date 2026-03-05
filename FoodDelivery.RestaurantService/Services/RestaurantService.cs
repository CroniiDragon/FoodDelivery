using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Interfaces;
using FoodDelivery.RestaurantService.Models;

namespace FoodDelivery.RestaurantService.Services;
public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _repo;
    public RestaurantService(IRestaurantRepository repo) => _repo = repo;

    public async Task<RestaurantResponseDto> CreateAsync(CreateRestaurantDto dto)
    {
        var r = new Restaurant
        {
            Name        = dto.Name,
            Address     = dto.Address,
            City        = dto.City,
            Cuisine     = dto.Cuisine,
            PhoneNumber = dto.PhoneNumber,
        };
        var saved = await _repo.AddAsync(r);
        return MapToDto(saved);
    }

    public async Task<RestaurantResponseDto?> GetByIdAsync(int id)
    {
        var r = await _repo.GetWithMenuAsync(id);
        return r == null ? null : MapToDto(r);
    }

    public async Task<IEnumerable<RestaurantResponseDto>> GetAllAsync()
        => (await _repo.GetAllAsync()).Select(MapToDto);

    public async Task<IEnumerable<RestaurantResponseDto>> GetOpenInCityAsync(string city)
        => (await _repo.GetByCityAsync(city)).Where(r => r.IsOpen).Select(MapToDto);

    public async Task<bool> DeleteAsync(int id)
    {
        if (!await _repo.ExistsAsync(id)) return false;
        await _repo.DeleteAsync(id);
        return true;
    }

    private static RestaurantResponseDto MapToDto(Restaurant r) => new()
    {
        Id            = r.Id,
        Name          = r.Name,
        Address       = r.Address,
        City          = r.City,
        Cuisine       = r.Cuisine,
        IsOpen        = r.IsOpen,
        MenuItemCount = r.MenuItems?.Count ?? 0,
    };
}

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _repo;
    public MenuItemService(IMenuItemRepository repo) => _repo = repo;

    public async Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto)
    {
        MenuItem item = dto.ItemType == "Drink"
            ? new DrinkItem
            {
                VolumeInLiters = dto.VolumeInLiters ?? 0.5,
                IsAlcoholic    = dto.IsAlcoholic ?? false,
            }
            : new FoodItem
            {
                Calories     = dto.Calories ?? 0,
                IsVegetarian = dto.IsVegetarian ?? false,
                IsVegan      = dto.IsVegan ?? false,
            };

        item.Name         = dto.Name;
        item.Description  = dto.Description;
        item.BasePrice    = dto.BasePrice;
        item.Category     = dto.Category;
        item.RestaurantId = dto.RestaurantId;

        var saved = await _repo.AddAsync(item);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<MenuItemResponseDto>> GetByRestaurantAsync(int restaurantId)
        => (await _repo.GetByRestaurantAsync(restaurantId)).Select(MapToDto);

    public async Task<bool> ToggleAvailabilityAsync(int menuItemId)
    {
        var item = await _repo.GetByIdAsync(menuItemId);
        if (item == null) return false;
        item.IsAvailable = !item.IsAvailable;
        await _repo.UpdateAsync(item);
        return true;
    }

    private static MenuItemResponseDto MapToDto(MenuItem m) => new()
    {
        Id           = m.Id,
        Name         = m.Name,
        Description  = m.Description,
        FinalPrice   = m.GetFinalPrice(),
        ItemType     = m.GetItemType(),
        Category     = m.Category,
        IsAvailable  = m.IsAvailable,
        RestaurantId = m.RestaurantId,
    };
}
