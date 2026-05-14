using FoodDelivery.NotificationService.ChainOfResponsibility;
using FoodDelivery.NotificationService.DTOs;
using FoodDelivery.NotificationService.Factories;
using FoodDelivery.NotificationService.Interfaces;
using FoodDelivery.NotificationService.Models;

namespace FoodDelivery.NotificationService.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository                  _repo;
    private readonly IEnumerable<INotificationChannel>        _channels;
    private readonly IEnumerable<INotificationMessageFactory> _factories;
    private readonly NotificationHandler                      _chain;

    public NotificationService(
        INotificationRepository                  repo,
        IEnumerable<INotificationChannel>        channels,
        IEnumerable<INotificationMessageFactory> factories)
    {
        _repo      = repo;
        _channels  = channels;
        _factories = factories;
        // Chain of Responsibility — built once, reused for every send
        _chain     = NotificationChainBuilder.Build();
    }

    public async Task<NotificationResponseDto> SendAsync(SendNotificationDto dto)
    {
        // Chain of Responsibility — validate before doing anything
        var validation = await _chain.HandleAsync(dto);
        if (!validation.IsValid)
            throw new InvalidOperationException($"Validation failed: {validation.Error}");

        // Factory Method — prepare message formatted for its channel
        var factory  = _factories.FirstOrDefault(f =>
            f.ChannelName.Equals(dto.Channel, StringComparison.OrdinalIgnoreCase));
        var prepared = factory?.Create(dto.RecipientId, dto.Message);
        var content  = prepared?.FormattedContent ?? dto.Message;

        // Send through channel (could be real EmailServiceAdapter or default channel)
        var channel = _channels.FirstOrDefault(c =>
            c.ChannelName.Equals(dto.Channel, StringComparison.OrdinalIgnoreCase));

        var isSent = false;
        if (channel != null)
        {
            isSent = await channel.SendAsync(dto.RecipientId, content);
            if (prepared != null)
                Console.WriteLine(prepared.GetSummary());
        }

        var notification = new Notification
        {
            RecipientId   = dto.RecipientId,
            RecipientType = dto.RecipientType,
            Channel       = dto.Channel,
            Message       = content,
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

// Channels — unchanged from Lab 1, now receive FORMATTED content from factories
public class EmailChannel : INotificationChannel
{
    public string ChannelName => "Email";
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        Console.WriteLine($"[EMAIL] → #{recipientId}: {formattedContent[..Math.Min(80, formattedContent.Length)]}...");
        await Task.CompletedTask;
        return true;
    }
}

public class SmsChannel : INotificationChannel
{
    public string ChannelName => "SMS";
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        Console.WriteLine($"[SMS] → #{recipientId}: {formattedContent}");
        await Task.CompletedTask;
        return true;
    }
}

public class PushChannel : INotificationChannel
{
    public string ChannelName => "Push";
    public async Task<bool> SendAsync(int recipientId, string formattedContent)
    {
        Console.WriteLine($"[PUSH] → #{recipientId}: {formattedContent}");
        await Task.CompletedTask;
        return true;
    }
}
