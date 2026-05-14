using FoodDelivery.NotificationService.DTOs;

namespace FoodDelivery.NotificationService.ChainOfResponsibility;

// Handler base class — each handler processes or passes to next
public abstract class NotificationHandler
{
    private NotificationHandler? _next;

    public NotificationHandler SetNext(NotificationHandler next)
    {
        _next = next;
        return next; // allows chaining: h1.SetNext(h2).SetNext(h3)
    }

    // Try to handle; if can't, pass to next
    public async Task<ValidationResult> HandleAsync(SendNotificationDto dto)
    {
        var result = await ValidateAsync(dto);
        if (!result.IsValid) return result;

        if (_next != null)
            return await _next.HandleAsync(dto);

        return ValidationResult.Ok();
    }

    protected abstract Task<ValidationResult> ValidateAsync(SendNotificationDto dto);
}

// Handler 1: checks required fields are present
public class RequiredFieldsHandler : NotificationHandler
{
    protected override Task<ValidationResult> ValidateAsync(SendNotificationDto dto)
    {
        if (dto.RecipientId <= 0)
            return Task.FromResult(ValidationResult.Fail("RecipientId must be positive."));

        if (string.IsNullOrWhiteSpace(dto.Message))
            return Task.FromResult(ValidationResult.Fail("Message cannot be empty."));

        Console.WriteLine("[Chain] RequiredFields → OK");
        return Task.FromResult(ValidationResult.Ok());
    }
}

// Handler 2: checks channel is supported
public class ChannelSupportHandler : NotificationHandler
{
    private static readonly string[] SupportedChannels = ["Email", "SMS", "Push"];

    protected override Task<ValidationResult> ValidateAsync(SendNotificationDto dto)
    {
        if (!SupportedChannels.Contains(dto.Channel))
            return Task.FromResult(ValidationResult.Fail($"Channel '{dto.Channel}' is not supported."));

        Console.WriteLine("[Chain] ChannelSupport → OK");
        return Task.FromResult(ValidationResult.Ok());
    }
}

// Handler 3: checks message length per channel
public class MessageLengthHandler : NotificationHandler
{
    protected override Task<ValidationResult> ValidateAsync(SendNotificationDto dto)
    {
        var limit = dto.Channel switch
        {
            "SMS"  => 160,
            "Push" => 200,
            _      => 5000
        };

        if (dto.Message.Length > limit)
            return Task.FromResult(ValidationResult.Fail(
                $"Message too long for {dto.Channel}: {dto.Message.Length}/{limit} chars."));

        Console.WriteLine("[Chain] MessageLength → OK");
        return Task.FromResult(ValidationResult.Ok());
    }
}

// Handler 4: rate limiting — max 5 notifications per recipient per minute (in-memory)
public class RateLimitHandler : NotificationHandler
{
    private static readonly Dictionary<int, (int Count, DateTime Window)> _counters = new();
    private const int MaxPerMinute = 5;

    protected override Task<ValidationResult> ValidateAsync(SendNotificationDto dto)
    {
        var now = DateTime.UtcNow;

        if (_counters.TryGetValue(dto.RecipientId, out var entry))
        {
            if ((now - entry.Window).TotalMinutes < 1)
            {
                if (entry.Count >= MaxPerMinute)
                    return Task.FromResult(ValidationResult.Fail(
                        $"Rate limit exceeded for recipient #{dto.RecipientId}."));

                _counters[dto.RecipientId] = (entry.Count + 1, entry.Window);
            }
            else
            {
                _counters[dto.RecipientId] = (1, now);
            }
        }
        else
        {
            _counters[dto.RecipientId] = (1, now);
        }

        Console.WriteLine("[Chain] RateLimit → OK");
        return Task.FromResult(ValidationResult.Ok());
    }
}

public record ValidationResult(bool IsValid, string Error = "")
{
    public static ValidationResult Ok()            => new(true);
    public static ValidationResult Fail(string e)  => new(false, e);
}

// Builder for the chain
public static class NotificationChainBuilder
{
    public static NotificationHandler Build()
    {
        var required  = new RequiredFieldsHandler();
        var channel   = new ChannelSupportHandler();
        var length    = new MessageLengthHandler();
        var rateLimit = new RateLimitHandler();

        required.SetNext(channel).SetNext(length).SetNext(rateLimit);

        return required; // entry point of the chain
    }
}
