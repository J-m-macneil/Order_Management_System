namespace Application.Features.AuditLogs.DTOs;

public class AuditLogSummaryDto
{
    public string LatestActivityText { get; set; } = "No activity";
    public DateTime? LatestActivityTime { get; set; }
    public int FailedActionCount { get; set; }
    public int SystemActionCount { get; set; }
}
