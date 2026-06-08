using Application.Features.AuditLogs.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.AuditLogs.Queries.GetAuditLogSummary;

public class GetAuditLogSummaryQueryHandler : IRequestHandler<GetAuditLogSummaryQuery, AuditLogSummaryDto>
{
    private readonly IAuditLogRepository _repo;

    public GetAuditLogSummaryQueryHandler(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<AuditLogSummaryDto> Handle(GetAuditLogSummaryQuery request, CancellationToken ct)
    {
        var summary = await _repo.GetSummaryAsync(
            request.SearchTerm,
            request.EntityType,
            request.Action,
            request.EntityId,
            request.PerformedByUserId,
            request.From,
            request.To,
            ct);

        return new AuditLogSummaryDto
        {
            LatestActivityText = summary.LatestActivity == null
                ? "No activity"
                : GetEventLabel(summary.LatestActivity.EntityType, summary.LatestActivity.Action),
            LatestActivityTime = summary.LatestActivity?.PerformedAt,
            FailedActionCount = summary.FailedActionCount,
            SystemActionCount = summary.SystemActionCount
        };
    }

    private static string GetEventLabel(string entityType, string action)
    {
        if (action.StartsWith("StatusChanged:"))
        {
            var status = action.Split(':')[1];
            var readableStatus = FormatName(status);

            if (entityType == "Order" && status == "Approved")
            {
                return "Order approved";
            }

            if (entityType == "Order" && (status == "AwaitingDispatch" || status == "Awaiting Dispatch"))
            {
                return "Order awaiting dispatch";
            }

            return $"{FormatName(entityType)} moved to {readableStatus}";
        }

        return action switch
        {
            "Created" => $"{FormatName(entityType)} created",
            "Updated" => $"{FormatName(entityType)} updated",
            "Deleted" => $"{FormatName(entityType)} deleted",
            "Generated" => $"{FormatName(entityType)} generated",
            "Sent" => $"{FormatName(entityType)} sent",
            "Completed" => $"{FormatName(entityType)} completed",
            "Failed" => $"{FormatName(entityType)} failed",
            "RetryQueued" => $"{FormatName(entityType)} retry queued",
            _ => $"{FormatName(entityType)} {action}"
        };
    }

    private static string FormatName(string value)
    {
        return string.Concat(value.Select((character, index) =>
            index > 0 && char.IsUpper(character) && char.IsLower(value[index - 1])
                ? $" {character}"
                : character.ToString()));
    }
}
