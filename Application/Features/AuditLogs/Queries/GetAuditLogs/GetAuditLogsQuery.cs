using Application.Common.Models;
using Application.Features.AuditLogs.DTOs;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetAuditLogs;

public class GetAuditLogsQuery : PaginationQuery, IRequest<PagedResult<AuditLogDto>>
{
    public string? SearchTerm { get; set; }
    public string? EntityType { get; set; }
    public string? Action { get; set; }
    public int? EntityId { get; set; }
    public int? PerformedByUserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
