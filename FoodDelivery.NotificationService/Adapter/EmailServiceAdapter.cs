using FoodDelivery.NotificationService.Interfaces;

namespace FoodDelivery.NotificationService.Adapter;

// Incompatible external service interface (e.g. SendGrid, Mailgun)
// We cannot modify this class — it comes from a third-party library
public class ExternalEmailService
{
    public void SendEmail(string toAddress, string subject, string htmlBody)
    {
        // In production: calls SendGrid/Mailgun API
        Console.WriteLine($"[ExternalEmailService] TO: {toAddress} | SUBJECT: {subject}");
        Console.WriteLine($"[ExternalEmailService] BODY: {htmlBody[..Math.Min(80, htmlBody.Length)]}...");
    }

    public string GetServiceName() => "ExternalEmailService v2";
}

// Adapter: wraps ExternalEmailService and exposes INotificationChannel
// Now NotificationService can use the external service without any changes
public class EmailServiceAdapter : INotificationChannel
{
    private readonly ExternalEmailService _external;
    private readonly string _defaultDomain;

    public string ChannelName => "Email";

    public EmailServiceAdapter(ExternalEmailService external, string defaultDomain = "fooddelivery.md")
    {
        _external      = external;
        _defaultDomain = defaultDomain;
    }

    public async Task<bool> SendAsync(int recipientId, string message)
    {
        // Adapt: INotificationChannel uses (recipientId, message)
        // ExternalEmailService needs (toAddress, subject, htmlBody)
        var toAddress = $"user{recipientId}@{_defaultDomain}";
        var subject   = ExtractSubject(message);
        var htmlBody  = WrapHtml(message);

        _external.SendEmail(toAddress, subject, htmlBody);

        await Task.CompletedTask;
        return true;
    }

    private static string ExtractSubject(string message)
    {
        var words = message.Split(' ').Take(6);
        return string.Join(" ", words);
    }

    private static string WrapHtml(string message)
        => $"<html><body><p>{message}</p></body></html>";
}
