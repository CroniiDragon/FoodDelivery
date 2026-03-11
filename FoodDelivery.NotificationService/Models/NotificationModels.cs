using FoodDelivery.Shared.Models;

namespace FoodDelivery.NotificationService.Models;

public class Notification : BaseEntity
{
    public int    RecipientId   { get; set; }
    public string RecipientType { get; set; } = string.Empty; // "Customer" / "Courier"
    public string Channel       { get; set; } = string.Empty; // "SMS" / "Email" / "Push"
    public string Message       { get; set; } = string.Empty;
    public bool   IsSent        { get; set; } = false;
    public DateTime? SentAt     { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum NotificationChannel { SMS, Email, Push }
