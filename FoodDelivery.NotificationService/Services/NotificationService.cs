using FoodDelivery.NotificationService.DTOs;
using FoodDelivery.NotificationService.Factories;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.NotificationService.Models;

namespace FoodDelivery.NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository                  _repo;
    private readonly IEnumerable<INotificationChannel>        _channels;
    private readonly IEnumerable<INotificationMessageFactory> _messageFactories; // ← NOU

    public NotificationService(
        INotificationRepository                  repo,
        IEnumerable<INotificationChannel>        channels,
        IEnumerable<INotificationMessageFactory> messageFactories)
    {
        _repo             = repo;
        _channels         = channels;
        _messageFactories = messageFactories;
    }

    public async Task<NotificationResponseDto> SendAsync(SendNotificationDto dto)
    {
        //FACTORY METHOD
        var messageFactory = _messageFactories.FirstOrDefault(f =>
            f.ChannelName.Equals(dto.Channel, StringComparison.OrdinalIgnoreCase));

        var preparedMessage = messageFactory?.Create(dto.RecipientId, dto.Message);

        var contentToSend = preparedMessage?.FormattedContent ?? dto.Message;

        //Send
        var channel = _channels.FirstOrDefault(c =>
            c.ChannelName.Equals(dto.Channel, StringComparison.OrdinalIgnoreCase));

        var isSent = false;
        if (channel != null)
        {
            isSent = await channel.SendAsync(dto.RecipientId, contentToSend);

            if (preparedMessage != null)
                Console.WriteLine(preparedMessage.GetSummary());
        }

        var notification = new Notification
        {
            RecipientId   = dto.RecipientId,
            RecipientType = dto.RecipientType,
            Channel       = dto.Channel,
            Message       = contentToSend, //formatat
            IsSent        = isSent,
            SentAt        = isSent ? DateTime.UtcNow : null,
        };

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
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        // In production: SMTP client send formattedContent (HTML comleted)
        Console.WriteLine($"[EMAIL SEND] → Recipient #{recipientId} | {formattedContent.Length} chars HTML");
        await Task.CompletedTask;
        return true;
    }
}

public class SmsChannel : INotificationChannel
{
    public string ChannelName => "SMS";
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        Console.WriteLine($"[SMS SEND] → Recipient #{recipientId}: '{formattedContent}'");
        await Task.CompletedTask;
        return true;
    }
}

public class PushChannel : INotificationChannel
{
    public string ChannelName => "Push";
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        Console.WriteLine($"[PUSH SEND] → Recipient #{recipientId}: {formattedContent}");
        await Task.CompletedTask;
        return true;
    }
}