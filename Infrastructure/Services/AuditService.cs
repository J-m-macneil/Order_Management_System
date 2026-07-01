using System.Text.Json;
using Application.Common.Interfaces;
using Application.Interfaces;
using Infrastructure.Persistence.Context;

namespace Infrastructure.Services;

public class AuditService : IAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AuditService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public void AddUserAction(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes)
    {
        AddEntry(
            entityType,
            entityId,
            action,
            oldValues,
            newValues,
            notes,
            _currentUser.UserId);
    }

    public void AddSystemAction(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes)
    {
        AddEntry(
            entityType,
            entityId,
            action,
            oldValues,
            newValues,
            notes,
            performedByUserId: null);
    }

    public async Task LogAsync(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes,
        CancellationToken ct)
    {
        AddUserAction(entityType, entityId, action, oldValues, newValues, notes);

        await _db.SaveChangesAsync(ct);
    }

    private void AddEntry(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes,
        int? performedByUserId)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = performedByUserId,
            PerformedAt = DateTime.UtcNow,
            OldValuesJson = oldValues == null
                ? null
                : JsonSerializer.Serialize(oldValues, JsonOptions),
            NewValuesJson = newValues == null
                ? null
                : JsonSerializer.Serialize(newValues, JsonOptions),
            Notes = notes
        });
    }
}
