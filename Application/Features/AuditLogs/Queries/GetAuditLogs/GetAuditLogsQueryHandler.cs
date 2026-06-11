using Application.Common.Models;
using Application.Features.AuditLogs.DTOs;
using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQueryHandler
    : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IAuditLogRepository _repo;
    private readonly IAuditChangeFormatter _changeFormatter;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository repo,
        IAuditChangeFormatter changeFormatter)
    {
        _repo = repo;
        _changeFormatter = changeFormatter;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(
        GetAuditLogsQuery request,
        CancellationToken ct)
    {
        var totalCount = await _repo.CountAsync(
            request.SearchTerm,
            request.EntityType,
            request.Action,
            request.EntityId,
            request.PerformedByUserId,
            request.From,
            request.To,
            ct);

        var logs = await _repo.GetPagedAsync(
            request.SearchTerm,
            request.EntityType,
            request.Action,
            request.EntityId,
            request.PerformedByUserId,
            request.From,
            request.To,
            request.Skip,
            request.PageSize,
            ct);

        var items = logs.Select(x => new AuditLogDto
        {
            AuditLogId = x.AuditLogId,
            EntityType = x.EntityType,
            EntityId = x.EntityId,
            Action = x.Action,
            PerformedByUserId = x.PerformedByUserId,
            PerformedByUserName = x.PerformedByUser?.FullName,
            PerformedAt = x.PerformedAt,
            OldValuesJson = x.OldValuesJson,
            NewValuesJson = x.NewValuesJson,
            Notes = x.Notes,
            ChangeSummary = _changeFormatter.CreateChangeSummary(x.OldValuesJson, x.NewValuesJson)
        }).ToList();

        return new PagedResult<AuditLogDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
