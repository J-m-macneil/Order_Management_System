using Application.Common.Interfaces;
using Application.Interfaces;
using Domain.Enums;
using Domain.Repositories;
using Domain.Rules;
using MediatR;

namespace Application.Features.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommandHandler
    : IRequestHandler<ChangeOrderStatusCommand, bool>
{
    private readonly IOrderRepository _repo;
    private readonly ICurrentUserService _currentUser;
    private readonly IProcessingJobQueueService _jobQueue;
    private readonly INotificationService _notificationService;
    private readonly IAuditService _audit;
    private readonly IOrderReviewPolicy _reviewPolicy;

    public ChangeOrderStatusCommandHandler(
        IOrderRepository repo,
        ICurrentUserService currentUser,
        IProcessingJobQueueService jobQueue,
        INotificationService notificationService,
        IAuditService audit,
        IOrderReviewPolicy reviewPolicy)
    {
        _repo = repo;
        _currentUser = currentUser;
        _jobQueue = jobQueue;
        _notificationService = notificationService;
        _audit = audit;
        _reviewPolicy = reviewPolicy;
    }

    public async Task<bool> Handle(
        ChangeOrderStatusCommand request,
        CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new KeyNotFoundException(
                $"Order {request.OrderId} not found");

        var userId = _currentUser.UserId
            ?? throw new Exception("User not authenticated");

        var fromStatus = (OrderStatusEnum)order.OrderStatusId;
        var toStatus = (OrderStatusEnum)request.StatusId;

        if (!OrderStatusTransitions.CanTransition(fromStatus, toStatus, _currentUser.Roles))
        {
            throw new InvalidOperationException(
                $"Invalid transition from {fromStatus} to {toStatus}");
        }

        if (RequiresReason(fromStatus, toStatus) && string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new InvalidOperationException(
                $"A reason is required to move an order from {fromStatus} to {toStatus}.");
        }

        if (toStatus == OrderStatusEnum.Approved &&
            fromStatus != OrderStatusEnum.PendingReview &&
            _reviewPolicy.RequiresManualReview(order))
        {
            throw new InvalidOperationException(
                "Orders containing restricted products must be reviewed before approval.");
        }

        order.ChangeStatus(request.StatusId, userId, request.Reason);

        await _repo.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "Order",
            order.OrderId,
            $"StatusChanged:{toStatus}",
            new
            {
                statusId = (int)fromStatus,
                status = fromStatus.ToString()
            },
            new
            {
                statusId = (int)toStatus,
                status = toStatus.ToString(),
                reason = request.Reason
            },
            $"Order status changed from {fromStatus} to {toStatus}.",
            ct);

        if (toStatus == OrderStatusEnum.Submitted)
        {
            await _notificationService.CreateOrderSubmittedNotificationAsync(order.OrderId, ct);
        }

        if (toStatus == OrderStatusEnum.Approved)
        {
            await _jobQueue.QueueApprovalJobsAsync(order.OrderId);
        }

        return true;
    }

    private static bool RequiresReason(OrderStatusEnum fromStatus, OrderStatusEnum toStatus)
    {
        return toStatus is OrderStatusEnum.Cancelled or OrderStatusEnum.Failed
            || (toStatus == OrderStatusEnum.Draft &&
                fromStatus is OrderStatusEnum.Submitted or OrderStatusEnum.PendingReview);
    }
}
