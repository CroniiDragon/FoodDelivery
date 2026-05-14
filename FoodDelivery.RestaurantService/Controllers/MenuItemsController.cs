using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Interfaces;
using FoodDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.RestaurantService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemsController : ControllerBase
{
    private readonly IMenuItemService _service;

    public MenuItemsController(IMenuItemService service) => _service = service;

    [HttpGet("restaurant/{restaurantId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuItemResponseDto>>>> GetByRestaurant(int restaurantId)
    {
        var items = await _service.GetByRestaurantAsync(restaurantId);
        return Ok(ApiResponse<IEnumerable<MenuItemResponseDto>>.Ok(items));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuItemResponseDto>>> Create([FromBody] CreateMenuItemDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(ApiResponse<MenuItemResponseDto>.Ok(created));
    }

    [HttpPatch("{id}/toggle-availability")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleAvailability(int id)
    {
        var result = await _service.ToggleAvailabilityAsync(id);
        return Ok(ApiResponse<bool>.Ok(result));
    }
}
