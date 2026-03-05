using FoodDelivery.NotificationService.DTOs;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.NotificationService.Models;
using FoodDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FoodDelivery.NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IEnumerable<INotificationChannel> _channels;

    public NotificationService(INotificationRepository repo, IEnumerable<INotificationChannel> channels)
    {
        _repo     = repo;
        _channels = channels;
    }

    public async Task<NotificationResponseDto> SendAsync(SendNotificationDto dto)
    {
        var channel = _channels.FirstOrDefault(c =>
            c.ChannelName.Equals(dto.Channel, StringComparison.OrdinalIgnoreCase));

        var notification = new Notification
        {
            RecipientId   = dto.RecipientId,
            RecipientType = dto.RecipientType,
            Channel       = dto.Channel,
            Message       = dto.Message,
        };

        if (channel != null)
        {
            notification.IsSent = await channel.SendAsync(dto.RecipientId, dto.Message);
            notification.SentAt = notification.IsSent ? DateTime.UtcNow : null;
        }

        var saved = await _repo.AddAsync(notification);
        return MapToDto(saved);
    }

    public async Task<IEnumerable<NotificationResponseDto>> GetByRecipientAsync(int recipientId)
        => (await _repo.GetByRecipientAsync(recipientId)).Select(MapToDto);

    private static NotificationResponseDto MapToDto(Notification n) => new()
    {
        Id          = n.Id,
        RecipientId = n.RecipientId,
        Channel     = n.Channel,
        Message     = n.Message,
        IsSent      = n.IsSent,
        CreatedAt   = n.CreatedAt,
    };
}

public class EmailChannel : INotificationChannel
{
    public string ChannelName => "Email";
    public async Task<bool> SendAsync(int recipientId, string message)
    {
        Console.WriteLine($"[EMAIL] → Recipient {recipientId}: {message}");
        await Task.CompletedTask;
        return true;
    }
}

public class SmsChannel : INotificationChannel
{
    public string ChannelName => "SMS";
    public async Task<bool> SendAsync(int recipientId, string message)
    {
        Console.WriteLine($"[SMS] → Recipient {recipientId}: {message}");
        await Task.CompletedTask;
        return true;
    }
}

public class PushChannel : INotificationChannel
{
    public string ChannelName => "Push";
    public async Task<bool> SendAsync(int recipientId, string message)
    {
        Console.WriteLine($"[PUSH] → Recipient {recipientId}: {message}");
        await Task.CompletedTask;
        return true;
    }
}