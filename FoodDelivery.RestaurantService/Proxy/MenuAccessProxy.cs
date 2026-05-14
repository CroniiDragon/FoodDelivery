using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Interfaces;

namespace FoodDelivery.RestaurantService.Proxy;

// Proxy: sits in front of IMenuItemService and checks if the restaurant
// is open before allowing menu access. Client code uses the same interface.
public class MenuAccessProxy : IMenuItemService
{
    private readonly IMenuItemService    _real;
    private readonly IRestaurantRepository _restaurantRepo;

    public MenuAccessProxy(IMenuItemService real, IRestaurantRepository restaurantRepo)
    {
        _real           = real;
        _restaurantRepo = restaurantRepo;
    }

    public async Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto)
        => await _real.CreateAsync(dto);

    // Proxy intercepts: only returns available items if restaurant is open
    public async Task<IEnumerable<MenuItemResponseDto>> GetByRestaurantAsync(int restaurantId)
    {
        var restaurant = await _restaurantRepo.GetByIdAsync(restaurantId);

        if (restaurant == null)
            throw new KeyNotFoundException($"Restaurant {restaurantId} not found.");

        if (!restaurant.IsOpen)
        {
            Console.WriteLine($"[Proxy] Restaurant #{restaurantId} is closed — returning empty menu.");
            return [];
        }

        return await _real.GetByRestaurantAsync(restaurantId);
    }

    public async Task<bool> ToggleAvailabilityAsync(int menuItemId)
        => await _real.ToggleAvailabilityAsync(menuItemId);
}
