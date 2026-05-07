using Domain.Entities.Orders;

public class Notification
{
    public int NotificationId { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string RecipientEmail { get; set; } = null!;
    public string NotificationType { get; set; } = null!;
    public string Subject { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }

    public string Status { get; set; } = null!;
    public string? FailureReason { get; set; }
}