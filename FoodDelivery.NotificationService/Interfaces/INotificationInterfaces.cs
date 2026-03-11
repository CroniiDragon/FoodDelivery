using FoodDelivery.Shared.Interfaces;
using FoodDelivery.NotificationService.DTOs;
using FoodDelivery.NotificationService.Models;

namespace FoodDelivery.NotificationService.Interfaces;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IEnumerable<Notification>> GetByRecipientAsync(int recipientId);
}

public interface INotificationService
{
    Task<NotificationResponseDto> SendAsync(SendNotificationDto dto);
    Task<IEnumerable<NotificationResponseDto>> GetByRecipientAsync(int recipientId);
}

public interface INotificationChannel
{
    string ChannelName { get; }
    Task<bool> SendAsync(int recipientId, string message);
}
