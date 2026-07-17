using Domain.Models;
using Domain.Repositories;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _db;

    public DashboardRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardMetrics> GetMetricsAsync(CancellationToken ct)
    {
        var activeStatusIds = new[]
        {
            (int)OrderStatusEnum.Submitted,
            (int)OrderStatusEnum.PendingReview,
            (int)OrderStatusEnum.Approved,
            (int)OrderStatusEnum.InProcessing,
            (int)OrderStatusEnum.AwaitingDispatch
        };
        var failedStatusId = (int)OrderStatusEnum.Failed;

        var totalOrders = await _db.Orders.CountAsync(o => o.DeletedAt == null, ct);

        var activeOrders = await _db.Orders
            .CountAsync(o => o.DeletedAt == null && activeStatusIds.Contains(o.OrderStatusId), ct);

        var failedOrders = await _db.Orders
            .CountAsync(o => o.DeletedAt == null && o.OrderStatusId == failedStatusId, ct);

        var totalValue = await _db.Orders
            .Where(o => o.DeletedAt == null)
            .SumAsync(o => o.TotalAmount, ct);

        var ordersByStatus = await _db.OrderStatuses
            .AsNoTracking()
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new StatusCount
            {
                Status = s.Name,
                Count = _db.Orders.Count(o =>
                    o.DeletedAt == null &&
                    o.OrderStatusId == s.OrderStatusId)
            })
            .ToListAsync(ct);

        var topCustomers = await _db.Orders
            .Include(o => o.Customer)
            .Where(o => o.DeletedAt == null)
            .GroupBy(o => o.Customer.CompanyName)
            .Select(g => new TopCustomer
            {
                Name = g.Key,
                Orders = g.Count()
            })
            .OrderByDescending(x => x.Orders)
            .Take(5)
            .ToListAsync(ct);

        var recentFailures = await _db.Orders
            .AsNoTracking()
            .Where(o =>
                o.DeletedAt == null &&
                o.OrderStatusHistory.Any(h => h.ToStatusId == failedStatusId))
            .Select(o => new
            {
                o.OrderId,
                o.OrderNumber,
                Customer = o.Customer.CompanyName,
                o.OrderStatusId,
                LatestFailure = o.OrderStatusHistory
                    .Where(h => h.ToStatusId == failedStatusId)
                    .OrderByDescending(h => h.ChangedAt)
                    .Select(h => new { h.Reason, h.ChangedAt })
                    .First()
            })
            .OrderByDescending(x => x.LatestFailure.ChangedAt)
            .Take(5)
            .Select(x => new RecentFailure
            {
                OrderId = x.OrderId,
                OrderNumber = x.OrderNumber,
                Customer = x.Customer,
                Reason = x.LatestFailure.Reason ?? "No failure reason recorded.",
                Date = x.LatestFailure.ChangedAt,
                RequiresAction = x.OrderStatusId == failedStatusId
            })
            .ToListAsync(ct);

        var priorityOrders = await _db.Orders
            .Include(o => o.Customer)
            .Where(o => o.DeletedAt == null && o.IsPriorityOrder)
            .OrderBy(o => o.RequestedDeliveryDate)
            .Take(5)
            .Select(o => new PriorityOrder
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,
                Customer = o.Customer.CompanyName,
                Priority = "High",
                DueDate = o.RequestedDeliveryDate
            })
            .ToListAsync(ct);

        return new DashboardMetrics
        {
            Metrics = new Metrics
            {
                TotalOrders = totalOrders,
                ActiveOrders = activeOrders,
                FailedOrders = failedOrders,
                TotalValue = totalValue
            },
            OrdersByStatus = ordersByStatus,
            TopCustomers = topCustomers,
            RecentFailures = recentFailures,
            PriorityOrders = priorityOrders
        };
    }
}
