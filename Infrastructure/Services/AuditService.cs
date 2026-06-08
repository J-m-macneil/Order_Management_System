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

    public async Task LogAsync(
        string entityType,
        int entityId,
        string action,
        object? oldValues,
        object? newValues,
        string? notes,
        CancellationToken ct)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedByUserId = _currentUser.UserId,
            PerformedAt = DateTime.UtcNow,
            OldValuesJson = oldValues == null
                ? null
                : JsonSerializer.Serialize(oldValues, JsonOptions),
            NewValuesJson = newValues == null
                ? null
                : JsonSerializer.Serialize(newValues, JsonOptions),
            Notes = notes
        });

        await _db.SaveChangesAsync(ct);
    }
}
