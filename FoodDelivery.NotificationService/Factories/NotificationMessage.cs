namespace FoodDelivery.NotificationService.Factories;

// FACTORY METHOD: Each channel factory (Email, SMS, Push)
// creates and prepares its own notification message.

/// <summary>
/// Factory interface for creating notification messages
/// </summary>
public abstract class NotificationMessage
{
    public int    RecipientId { get; protected set; }
    public string RawText     { get; protected set; } = string.Empty;

    // Type-specific data populated by Prepare()
    public string FormattedContent { get; protected set; } = string.Empty;
    public string Subject          { get; protected set; } = string.Empty; // folosit de Email
    public bool   IsReady          { get; protected set; } = false;

    // FACTORY METHOD: Subclasses prepare the message for their channel.
    // Called by NotificationService without knowing the concrete type
    public abstract NotificationMessage Prepare(int recipientId, string rawText);

    public abstract string GetSummary();
}

// EmailMessage — formats the notification as an HTML email
public class EmailMessage : NotificationMessage
{
    public override NotificationMessage Prepare(int recipientId, string rawText)
    {
        RecipientId = recipientId;
        RawText     = rawText;

        // Extract the subject: first 8 words, max 60 characters
        var words = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var subjectWords = words.Take(8);
        Subject = string.Join(" ", subjectWords);
        if (Subject.Length > 60) Subject = Subject[..57] + "...";

        // Format content as HTML with header and footer
        FormattedContent = $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"/></head>
            <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
              <div style="background:#f97316; padding:20px; border-radius:8px 8px 0 0;">
                <h2 style="color:white; margin:0;">FoodDelivery Platform</h2>
              </div>
              <div style="padding:24px; background:#fff; border:1px solid #e5e7eb;">
                <p style="font-size:16px; color:#111;">{rawText}</p>
              </div>
              <div style="padding:12px; background:#f9fafb; border-radius:0 0 8px 8px;">
                <p style="font-size:12px; color:#6b7280; margin:0;">
                  Acest email a fost trimis automat catre utilizatorul #{recipientId}.
                  Va rugam nu raspundeti la acest email.
                </p>
              </div>
            </body>
            </html>
            """;

        IsReady = true;
        return this;
    }

    public override string GetSummary()
        => $"[EMAIL] Subject: '{Subject}' → Recipient #{RecipientId} | {FormattedContent.Length} chars HTML";
}

// SmsMessage — formats the notification for SMS (max 160 chars)

public class SmsMessage : NotificationMessage
{
    public int CharacterCount { get; private set; }
    public bool WasTruncated  { get; private set; }

    private static readonly (string From, string To)[] DiacriticMap =
    [
        ("ă","a"), ("â","a"), ("î","i"), ("ș","s"), ("ț","t"),
        ("Ă","A"), ("Â","A"), ("Î","I"), ("Ș","S"), ("Ț","T"),
        ("ş","s"), ("ţ","t"),  // variants of diacritics??
    ];

    public override NotificationMessage Prepare(int recipientId, string rawText)
    {
        RecipientId = recipientId;
        RawText     = rawText;

        //remove diacritics for GSM network compatibility
        var normalized = rawText;
        foreach (var (from, to) in DiacriticMap)
            normalized = normalized.Replace(from, to);

        // remove unsupported special characters
        normalized = new string(normalized
            .Where(c => c < 128) // doar ASCII standard
            .ToArray());

        //add FoodDelivery prefix
        const string prefix = "FD: ";
        const int maxLength = 160;
        var available = maxLength - prefix.Length;

        //truncate if it exceeds the limit
        if (normalized.Length > available)
        {
            normalized  = normalized[..(available - 3)] + "...";
            WasTruncated = true;
        }

        FormattedContent = prefix + normalized;
        CharacterCount   = FormattedContent.Length;
        Subject          = "SMS"; // SMS subject??
        IsReady          = true;

        return this;
    }

    public override string GetSummary()
        => $"[SMS] {CharacterCount}/160 chars{(WasTruncated ? " (trunchiat)" : "")} → Recipient #{RecipientId}: '{FormattedContent}'";
}

// PushMessage — formats the notification as a Push Notification
public class PushMessage : NotificationMessage
{
    public string Title { get; private set; } = string.Empty;
    public string Body  { get; private set; } = string.Empty;
    public string Icon  { get; private set; } = string.Empty;

    public override NotificationMessage Prepare(int recipientId, string rawText)
    {
        RecipientId = recipientId;
        RawText     = rawText;

        // Title: first 5 words, max 50 characters
        var words = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Title = string.Join(" ", words.Take(5));
        if (Title.Length > 50) Title = Title[..47] + "...";

        // full text truncated to 100 characters for mobile
        Body = rawText.Length > 100
            ? rawText[..97] + "..."
            : rawText;

        // Icon automatically selected based on keywords in the message
        Icon = rawText.ToLower() switch
        {
            var t when t.Contains("comanda") || t.Contains("order") => "🛍️",
            var t when t.Contains("livra")   || t.Contains("curier") => "🚴",
            var t when t.Contains("platit")  || t.Contains("plata")  => "💳",
            var t when t.Contains("anulat")  || t.Contains("cancel") => "❌",
            var t when t.Contains("gata")    || t.Contains("prepar") => "👨‍🍳",
            _ => "🔔"  // icon default
        };

        // FormattedContent = payload JSON for push notification service
        FormattedContent = $"{{\"title\":\"{Title}\",\"body\":\"{Body}\",\"icon\":\"{Icon}\"}}";
        Subject          = Title;
        IsReady          = true;

        return this;
    }

    public override string GetSummary()
        => $"[PUSH] {Icon} '{Title}' → Recipient #{RecipientId} | Body: '{Body}'";
}
