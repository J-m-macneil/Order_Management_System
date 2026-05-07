using Domain.Repositories;
using MediatR;

namespace Application.Features.Dashboard.Queries.GetDashboardMetrics;

public class GetDashboardMetricsQueryHandler
    : IRequestHandler<GetDashboardMetricsQuery, DashboardMetricsDto>
{
    private readonly IDashboardRepository _repo;

    public GetDashboardMetricsQueryHandler(IDashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardMetricsDto> Handle(GetDashboardMetricsQuery request, CancellationToken ct)
    {
        var result = await _repo.GetMetricsAsync(ct);

        return new DashboardMetricsDto
        {
            Metrics = new MetricsDto
            {
                TotalOrders = result.Metrics.TotalOrders,
                ActiveOrders = result.Metrics.ActiveOrders,
                FailedOrders = result.Metrics.FailedOrders,
                TotalValue = result.Metrics.TotalValue
            },
            OrdersByStatus = result.OrdersByStatus.Select(x => new StatusCountDto
            {
                Status = x.Status,
                Count = x.Count
            }).ToList(),
            TopCustomers = result.TopCustomers.Select(x => new TopCustomerDto
            {
                Name = x.Name,
                Orders = x.Orders
            }).ToList(),
            RecentFailures = result.RecentFailures.Select(x => new RecentFailureDto
            {
                OrderId = x.OrderId,
                OrderNumber = x.OrderNumber,
                Customer = x.Customer,
                Reason = x.Reason,
                Date = x.Date
            }).ToList(),
            PriorityOrders = result.PriorityOrders.Select(x => new PriorityOrderDto
            {
                OrderId = x.OrderId,
                OrderNumber = x.OrderNumber,
                Customer = x.Customer,
                Priority = x.Priority,
                DueDate = x.DueDate
            }).ToList()
        };
    }
}