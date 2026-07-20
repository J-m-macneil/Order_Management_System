using Application.Interfaces;
using Domain.Entities.Status;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderStatusWorkflowService : IOrderStatusWorkflowService
{
    private readonly AppDbContext _dbContext;
    private readonly IAuditService _auditService;

    public OrderStatusWorkflowService(
        AppDbContext dbContext,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _auditService = auditService;
    }

    public async Task MoveToStatusAsync(
        int orderId,
        OrderStatusEnum toStatus,
        string reason,
        CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders
            .FirstAsync(o => o.OrderId == orderId, cancellationToken);

        var oldStatusId = order.OrderStatusId;
        var toStatusId = (int)toStatus;

        if (oldStatusId == toStatusId)
        {
            return;
        }

        order.OrderStatusId = toStatusId;
        order.UpdatedAt = DateTime.UtcNow;

        if (toStatus == OrderStatusEnum.Failed)
        {
            order.FailureReason = reason;
        }

        _dbContext.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.OrderId,
            FromStatusId = oldStatusId,
            ToStatusId = toStatusId,
            ChangedByUserId = order.CreatedByUserId,
            ChangedAt = DateTime.UtcNow,
            Reason = reason
        });

        _auditService.AddSystemAction(
            "Order",
            order.OrderId,
            $"StatusChanged:{GetStatusName(toStatus)}",
            new { StatusId = oldStatusId },
            new
            {
                StatusId = toStatusId,
                Status = GetStatusName(toStatus)
            },
            reason);
    }

    private static string GetStatusName(OrderStatusEnum status)
    {
        return status switch
        {
            OrderStatusEnum.InProcessing => "In Processing",
            OrderStatusEnum.AwaitingDispatch => "Awaiting Dispatch",
            OrderStatusEnum.Completed => "Completed",
            OrderStatusEnum.Failed => "Failed",
            _ => status.ToString()
        };
    }
}
