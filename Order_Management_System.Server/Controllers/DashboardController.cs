using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin,Operations,Sales")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public DashboardController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        var activeStatusIds = new[] { 2, 3, 4, 5, 6 };

        var totalOrders = await _dbContext.Orders
            .CountAsync(o => o.DeletedAt == null);

        var activeOrders = await _dbContext.Orders
            .CountAsync(o => o.DeletedAt == null && activeStatusIds.Contains(o.OrderStatusId));

        var failedOrders = await _dbContext.Orders
            .CountAsync(o => o.DeletedAt == null && o.OrderStatusId == 8);

        var totalValue = await _dbContext.Orders
            .Where(o => o.DeletedAt == null)
            .SumAsync(o => o.TotalAmount);

        var ordersByStatus = await _dbContext.Orders
            .Include(o => o.OrderStatus)
            .Where(o => o.DeletedAt == null)
            .GroupBy(o => new
            {
                o.OrderStatusId,
                StatusName = o.OrderStatus.Name
            })
            .Select(g => new
            {
                Status = g.Key.StatusName,
                Count = g.Count()
            })
            .OrderBy(x => x.Status)
            .ToListAsync();

        var topCustomers = await _dbContext.Orders
            .Include(o => o.Customer)
            .Where(o => o.DeletedAt == null)
            .GroupBy(o => new
            {
                o.CustomerId,
                CustomerName = o.Customer.CompanyName
            })
            .Select(g => new
            {
                Name = g.Key.CustomerName,
                Orders = g.Count()
            })
            .OrderByDescending(x => x.Orders)
            .Take(5)
            .ToListAsync();

        var recentFailures = await _dbContext.Orders
            .Include(o => o.Customer)
            .Where(o => o.DeletedAt == null && o.OrderStatusId == 8)
            .OrderByDescending(o => o.UpdatedAt)
            .Take(5)
            .Select(o => new
            {
                o.OrderId,
                o.OrderNumber,
                Customer = o.Customer.CompanyName,
                Reason = o.FailureReason ?? "No failure reason recorded.",
                Date = o.UpdatedAt
            })
            .ToListAsync();

        var priorityOrders = await _dbContext.Orders
            .Include(o => o.Customer)
            .Where(o => o.DeletedAt == null && o.IsPriorityOrder && o.OrderStatusId != 7 && o.OrderStatusId != 9)
            .OrderBy(o => o.RequestedDeliveryDate)
            .Take(5)
            .Select(o => new
            {
                o.OrderId,
                o.OrderNumber,
                Customer = o.Customer.CompanyName,
                Priority = "High",
                DueDate = o.RequestedDeliveryDate
            })
            .ToListAsync();

        return Ok(new
        {
            Metrics = new
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
        });
    }
}