namespace Domain.Repositories;

public interface IAuditLogRepository
{
    Task<int> CountAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct);

    Task<(AuditLog? LatestActivity, int FailedActionCount, int SystemActionCount)> GetSummaryAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct);

    Task<List<AuditLog>> GetPagedAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        int skip,
        int take,
        CancellationToken ct);
}
