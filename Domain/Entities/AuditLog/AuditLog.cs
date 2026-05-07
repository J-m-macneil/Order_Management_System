using Domain.Entities.Identity;

public class AuditLog
{
    public int AuditLogId { get; set; }

    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }

    public string Action { get; set; } = null!;

    public int? PerformedByUserId { get; set; }
    public User? PerformedByUser { get; set; }

    public DateTime PerformedAt { get; set; }

    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? Notes { get; set; }
}