// ════ DTOs/NotificationDtos.cs ═════════════════════════════
namespace FoodDelivery.NotificationService.DTOs;

public class SendNotificationDto
{
    public int    RecipientId   { get; set; }
    public string RecipientType { get; set; } = string.Empty;
    public string Channel       { get; set; } = "Email";
    public string Message       { get; set; } = string.Empty;
}

public class NotificationResponseDto
{
    public int      Id            { get; set; }
    public int      RecipientId   { get; set; }
    public string   Channel       { get; set; } = string.Empty;
    public string   Message       { get; set; } = string.Empty;
    public bool     IsSent        { get; set; }
    public DateTime CreatedAt     { get; set; }
}
