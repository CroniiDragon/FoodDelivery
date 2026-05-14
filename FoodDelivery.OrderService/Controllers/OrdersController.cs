using FoodDelivery.OrderService.DTOs;
using FoodDelivery.OrderService.Facade;
using FoodDelivery.OrderService.Interfaces;
using FoodDelivery.OrderService.Strategy;
using FoodDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService   _service;
    private readonly OrderFacade     _facade;
    private readonly DeliveryContext _delivery;

    public OrdersController(IOrderService service, OrderFacade facade, DeliveryContext delivery)
    {
        _service  = service;
        _facade   = facade;
        _delivery = delivery;
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderResponseDto>>>> GetByCustomer(int customerId)
        => Ok(ApiResponse<IEnumerable<OrderResponseDto>>.Ok(await _service.GetByCustomerAsync(customerId)));

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetById(int id)
    {
        var o = await _service.GetByIdAsync(id);
        return o == null
            ? NotFound(ApiResponse<OrderResponseDto>.Fail($"Order {id} not found."))
            : Ok(ApiResponse<OrderResponseDto>.Ok(o));
    }

    // Standard create — uses Builder + Strategy + Observer internally
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponseDto>>> Create([FromBody] CreateOrderDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<OrderResponseDto>.Ok(created, "Order placed."));
    }

    // Facade endpoint — single call: create + payment + notification
    [HttpPost("place")]
    public async Task<ActionResult<ApiResponse<OrderFacadeResult>>> PlaceOrder([FromBody] CreateOrderDto dto)
    {
        var result = await _facade.PlaceOrderAsync(dto);
        return result.Success
            ? Ok(ApiResponse<OrderFacadeResult>.Ok(result, result.Message))
            : BadRequest(ApiResponse<OrderFacadeResult>.Fail(result.Message));
    }

    // Change delivery strategy at runtime
    [HttpPost("strategy/{strategy}")]
    public ActionResult<ApiResponse<string>> SetStrategy(string strategy)
    {
        IDeliveryStrategy s = strategy.ToLower() switch
        {
            "express"   => new ExpressDeliveryStrategy(),
            "citybased" => new CityDeliveryStrategy(),
            _           => new StandardDeliveryStrategy(),
        };
        _delivery.SetStrategy(s);
        return Ok(ApiResponse<string>.Ok(strategy, $"Strategy set to: {strategy}"));
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(int id, [FromBody] string status)
    {
        await _service.UpdateStatusAsync(id, status);
        return Ok(ApiResponse<bool>.Ok(true, "Status updated."));
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<ApiResponse<bool>>> Cancel(int id)
    {
        var ok = await _service.CancelAsync(id);
        return ok
            ? Ok(ApiResponse<bool>.Ok(true, "Order cancelled."))
            : BadRequest(ApiResponse<bool>.Fail("Cannot cancel order in current status."));
    }
}
