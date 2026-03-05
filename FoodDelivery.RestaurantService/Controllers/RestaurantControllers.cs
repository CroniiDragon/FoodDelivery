using FoodDelivery.Shared.DTOs;
using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.RestaurantService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _service;
    public RestaurantsController(IRestaurantService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RestaurantResponseDto>>>> GetAll()
        => Ok(ApiResponse<IEnumerable<RestaurantResponseDto>>.Ok(await _service.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r == null
            ? NotFound(ApiResponse<RestaurantResponseDto>.Fail($"Restaurant {id} negasit."))
            : Ok(ApiResponse<RestaurantResponseDto>.Ok(r));
    }

    [HttpGet("open/{city}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<RestaurantResponseDto>>>> GetOpenInCity(string city)
        => Ok(ApiResponse<IEnumerable<RestaurantResponseDto>>.Ok(await _service.GetOpenInCityAsync(city)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> Create([FromBody] CreateRestaurantDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<RestaurantResponseDto>.Ok(created, "Restaurant creat."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        return ok ? Ok(ApiResponse<bool>.Ok(true)) : NotFound(ApiResponse<bool>.Fail("Negasit."));
    }
}

[ApiController]
[Route("api/[controller]")]
public class MenuItemsController : ControllerBase
{
    private readonly IMenuItemService _service;
    public MenuItemsController(IMenuItemService service) => _service = service;

    [HttpGet("restaurant/{restaurantId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MenuItemResponseDto>>>> GetByRestaurant(int restaurantId)
        => Ok(ApiResponse<IEnumerable<MenuItemResponseDto>>.Ok(await _service.GetByRestaurantAsync(restaurantId)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MenuItemResponseDto>>> Create([FromBody] CreateMenuItemDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return Ok(ApiResponse<MenuItemResponseDto>.Ok(created, "Produs adaugat."));
    }

    [HttpPatch("{id}/toggle-availability")]
    public async Task<ActionResult<ApiResponse<bool>>> ToggleAvailability(int id)
    {
        var ok = await _service.ToggleAvailabilityAsync(id);
        return ok ? Ok(ApiResponse<bool>.Ok(true)) : NotFound(ApiResponse<bool>.Fail("Negasit."));
    }
}
