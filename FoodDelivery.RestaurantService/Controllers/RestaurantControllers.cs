using FoodDelivery.RestaurantService.DTOs;
using FoodDelivery.RestaurantService.Interfaces;
using FoodDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.RestaurantService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _service;

    public RestaurantController(IRestaurantService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<RestaurantResponseDto>>>> GetAll()
        => Ok(ApiResponse<IEnumerable<RestaurantResponseDto>>.Ok(await _service.GetAllAsync()));

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound(ApiResponse<RestaurantResponseDto>.Fail("Not found."))
                              : Ok(ApiResponse<RestaurantResponseDto>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> Create([FromBody] CreateRestaurantDto dto)
        => Ok(ApiResponse<RestaurantResponseDto>.Ok(await _service.CreateAsync(dto)));

    // Prototype — shallow clone (new branch, empty menu)
    [HttpPost("{id}/clone-branch")]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> CloneBranch(int id)
    {
        var result = await _service.CloneBranchAsync(id);
        return Ok(ApiResponse<RestaurantResponseDto>.Ok(result, "Branch created (shallow clone)."));
    }

    // Prototype — deep clone (seasonal copy with full menu)
    [HttpPost("{id}/clone-seasonal")]
    public async Task<ActionResult<ApiResponse<RestaurantResponseDto>>> CloneSeasonal(int id)
    {
        var result = await _service.CloneSeasonalAsync(id);
        return Ok(ApiResponse<RestaurantResponseDto>.Ok(result, "Seasonal copy created (deep clone)."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        => Ok(ApiResponse<bool>.Ok(await _service.DeleteAsync(id)));
}
