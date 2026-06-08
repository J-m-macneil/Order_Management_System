namespace Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes,
        CancellationToken ct);
}
