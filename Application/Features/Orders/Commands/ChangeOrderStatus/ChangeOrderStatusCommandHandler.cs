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

    public ChangeOrderStatusCommandHandler(
        IOrderRepository repo,
        ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
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

        order.ChangeStatus(
            request.StatusId,
            userId,
            request.Reason);

        await _repo.SaveChangesAsync(ct);

        return true;
    }
}