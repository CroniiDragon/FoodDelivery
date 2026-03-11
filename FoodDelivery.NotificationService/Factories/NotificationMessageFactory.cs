namespace FoodDelivery.NotificationService.Factories;

// FACTORY METHOD: Each channel factory (Email, SMS, Push)
// creates and prepares its own notification message.

/// <summary>
/// Factory interface for creating notification messages.
/// </summary>
public interface INotificationMessageFactory
{
    string ChannelName { get; }

    NotificationMessage Create(int recipientId, string rawText);
}

public class EmailMessageFactory : INotificationMessageFactory
{
    public string ChannelName => "Email";

    public NotificationMessage Create(int recipientId, string rawText)
    {
        return new EmailMessage().Prepare(recipientId, rawText);
    }
}

public class SmsMessageFactory : INotificationMessageFactory
{
    public string ChannelName => "SMS";

    public NotificationMessage Create(int recipientId, string rawText)
    {
        return new SmsMessage().Prepare(recipientId, rawText);
    }
}


public class PushMessageFactory : INotificationMessageFactory
{
    public string ChannelName => "Push";

    public NotificationMessage Create(int recipientId, string rawText)
    {
        return new PushMessage().Prepare(recipientId, rawText);
    }
}
