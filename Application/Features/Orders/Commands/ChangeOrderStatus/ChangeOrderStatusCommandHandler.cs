using Application.Common.Interfaces;
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

    public ChangeOrderStatusCommandHandler(
        IOrderRepository repo,
        ICurrentUserService currentUser,
        IProcessingJobQueueService jobQueue)
    {
        _repo = repo;
        _currentUser = currentUser;
        _jobQueue = jobQueue;
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

        if (!OrderStatusTransitions.CanTransition(fromStatus, toStatus))
        {
            throw new InvalidOperationException(
                $"Invalid transition from {fromStatus} to {toStatus}");
        }

        order.ChangeStatus(request.StatusId, userId, request.Reason);

        await _repo.SaveChangesAsync(ct);

        if (toStatus == OrderStatusEnum.Submitted)
        {
            await _jobQueue.QueueSubmissionJobsAsync(order.OrderId);
        }

        if (toStatus == OrderStatusEnum.Approved)
        {
            await _jobQueue.QueueApprovalJobsAsync(order.OrderId);
        }

        return true;
    }
}