using FoodDelivery.Shared.DTOs;
using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetByCustomer(int customerId)
        => Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(await _service.GetByCustomerAsync(customerId)));

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(int id)
    {
        var o = await _service.GetByIdAsync(id);
        return o == null
            ? NotFound(ApiResponse<OrderResponseDto>.Fail($"Comanda {id} negasita."))
            : Ok(ApiResponse<OrderResponseDto>.Ok(o));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Create([FromBody] CreateOrderDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<OrderResponseDto>.Ok(created, "Comanda plasata."));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] string status)
    {
        await _service.UpdateStatusAsync(id, status);
        return Ok(ApiResponse<bool>.Ok(true, "Status actualizat."));
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
    {
        var ok = await _service.CancelAsync(id);
        return ok
            ? Ok(ApiResponse<bool>.Ok(true, "Comanda anulata."))
            : BadRequest(ApiResponse<bool>.Fail("Comanda nu poate fi anulata in statusul curent."));
    }
}
