using FoodDelivery.NotificationService.DTOs;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.NotificationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    // POST api/notifications
    // Send notification, channel (Email / SMS / Push)
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NotificationResponseDto>>> Send(
        [FromBody] SendNotificationDto dto)
    {
        var result = await _service.SendAsync(dto);
        return Ok(ApiResponse<NotificationResponseDto>.Ok(result, "Notificare trimisa."));
    }

    // GET api/notifications/recipient/{recipientId}
    // Return all notification for this user
    [HttpGet("recipient/{recipientId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationResponseDto>>>> GetByRecipient(
        int recipientId)
    {
        var results = await _service.GetByRecipientAsync(recipientId);
        return Ok(ApiResponse<IEnumerable<NotificationResponseDto>>.Ok(results));
    }
}
