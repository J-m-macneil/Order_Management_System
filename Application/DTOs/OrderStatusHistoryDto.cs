namespace Application.DTOs;

public class OrderStatusHistoryDto
{
    public int OrderStatusHistoryId { get; set; }
    public string? FromStatusName { get; set; }
    public string ToStatusName { get; set; } = string.Empty;
    public string ChangedByUserName { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Reason { get; set; }
}