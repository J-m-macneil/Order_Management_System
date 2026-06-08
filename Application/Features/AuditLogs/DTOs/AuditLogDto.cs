namespace Application.Features.AuditLogs.DTOs;

public class AuditLogDto
{
    public int AuditLogId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Action { get; set; } = string.Empty;
    public int? PerformedByUserId { get; set; }
    public string? PerformedByUserName { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? Notes { get; set; }
}
