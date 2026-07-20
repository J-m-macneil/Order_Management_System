using MediatR;

namespace Application.Features.Dashboard.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQuery : IRequest<DashboardMetricsDto>
{
}