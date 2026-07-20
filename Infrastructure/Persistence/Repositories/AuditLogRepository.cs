using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;

    public AuditLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> CountAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.AuditLogs.AsNoTracking(),
                searchTerm,
                entityType,
                action,
                entityId,
                performedByUserId,
                from,
                to)
            .CountAsync(ct);
    }

    public async Task<(AuditLog? LatestActivity, int FailedActionCount, int SystemActionCount)> GetSummaryAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        CancellationToken ct)
    {
        var logs = ApplyFilters(
            _db.AuditLogs.AsNoTracking(),
            searchTerm,
            entityType,
            action,
            entityId,
            performedByUserId,
            from,
            to);

        var latestActivity = await logs
            .OrderByDescending(x => x.PerformedAt)
            .ThenByDescending(x => x.AuditLogId)
            .FirstOrDefaultAsync(ct);

        var failedActionCount = await logs.CountAsync(x =>
            x.Action.ToLower().Contains("failed") ||
            (x.Notes != null && x.Notes.ToLower().Contains("failed")), ct);

        var systemActionCount = await logs.CountAsync(x => x.PerformedByUserId == null, ct);

        return (latestActivity, failedActionCount, systemActionCount);
    }

    public async Task<List<AuditLog>> GetPagedAsync(
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to,
        int skip,
        int take,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.AuditLogs
                    .AsNoTracking()
                    .Include(x => x.PerformedByUser),
                searchTerm,
                entityType,
                action,
                entityId,
                performedByUserId,
                from,
                to)
            .OrderByDescending(x => x.PerformedAt)
            .ThenByDescending(x => x.AuditLogId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    private static IQueryable<AuditLog> ApplyFilters(
        IQueryable<AuditLog> query,
        string? searchTerm,
        string? entityType,
        string? action,
        int? entityId,
        int? performedByUserId,
        DateTime? from,
        DateTime? to)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.EntityType, pattern) ||
                EF.Functions.ILike(x.Action, pattern) ||
                (x.Notes != null && EF.Functions.ILike(x.Notes, pattern)) ||
                (x.PerformedByUser != null &&
                    (EF.Functions.ILike(x.PerformedByUser.FullName, pattern) ||
                     EF.Functions.ILike(x.PerformedByUser.Username, pattern) ||
                     EF.Functions.ILike(x.PerformedByUser.Email, pattern))));
        }

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(x => x.EntityType == entityType);

        if (!string.IsNullOrWhiteSpace(action))
        {
            var actionFilter = action.Trim();

            query = actionFilter.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                ? query.Where(x =>
                    x.Action.Contains(actionFilter) ||
                    (x.Notes != null && x.Notes.Contains(actionFilter)))
                : query.Where(x => x.Action.Contains(actionFilter));
        }

        if (entityId.HasValue)
            query = query.Where(x => x.EntityId == entityId.Value);

        if (performedByUserId.HasValue)
            query = query.Where(x => x.PerformedByUserId == performedByUserId.Value);

        if (from.HasValue)
            query = query.Where(x => x.PerformedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.PerformedAt <= to.Value);

        return query;
    }
}
