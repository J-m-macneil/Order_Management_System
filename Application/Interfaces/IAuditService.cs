namespace Application.Interfaces;

public interface IAuditService
{
    void AddUserAction(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes);

    void AddSystemAction(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes);

    Task LogAsync(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes,
        CancellationToken ct);
}
