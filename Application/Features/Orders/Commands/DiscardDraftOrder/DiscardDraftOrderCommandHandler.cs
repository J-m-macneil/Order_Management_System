using Application.Common.Interfaces;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Commands.DiscardDraftOrder;

public class DiscardDraftOrderCommandHandler : IRequestHandler<DiscardDraftOrderCommand>
{
    private readonly IOrderRepository _orders;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public DiscardDraftOrderCommandHandler(
        IOrderRepository orders,
        IAuditService audit,
        ICurrentUserService currentUser)
    {
        _orders = orders;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(DiscardDraftOrderCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found");

        if (!CanDiscardDraftOrder(_currentUser.Roles))
            throw new InvalidOperationException("Only Sales or Admin users can discard draft orders.");

        _ = _currentUser.UserId
            ?? throw new InvalidOperationException("User not authenticated.");

        var oldValues = CreateSnapshot(order);

        order.DiscardDraft();

        var newValues = CreateSnapshot(order);

        await _orders.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "Order",
            order.OrderId,
            "Discarded",
            oldValues,
            newValues,
            $"Draft order discarded: {order.OrderNumber}.",
            ct);
    }

    private static object CreateSnapshot(Order order)
    {
        return new
        {
            order.OrderId,
            order.OrderNumber,
            StatusId = order.OrderStatusId,
            Status = ((OrderStatusEnum)order.OrderStatusId).ToString(),
            order.DeletedAt,
            ItemCount = order.OrderItems.Count
        };
    }

    private static bool CanDiscardDraftOrder(IEnumerable<string> roles)
    {
        return roles.Any(role =>
            role.Equals("Sales", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase));
    }
}
