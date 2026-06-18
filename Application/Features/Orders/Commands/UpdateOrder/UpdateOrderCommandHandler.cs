using Application.Common.Interfaces;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand>
{
    private readonly IOrderRepository _orders;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public UpdateOrderCommandHandler(
        IOrderRepository orders,
        IAuditService audit,
        ICurrentUserService currentUser)
    {
        _orders = orders;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateOrderCommand request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found");

        if ((OrderStatusEnum)order.OrderStatusId != OrderStatusEnum.Draft)
            throw new InvalidOperationException("Only draft orders can be edited.");

        if (!CanEditDraftOrder(_currentUser.Roles))
            throw new InvalidOperationException("Only Sales or Admin users can edit draft orders.");

        if (request.Items.Count == 0)
            throw new InvalidOperationException("An order must have at least one order line.");

        var oldValues = CreateSnapshot(order);

        order.CustomerId = request.CustomerId;
        order.DeliveryAddressId = request.DeliveryAddressId;
        order.BillingAddressId = request.BillingAddressId;
        order.WarehouseId = request.WarehouseId;
        order.CarrierId = request.CarrierId;
        order.ProjectId = request.ProjectId;
        order.RequestedDeliveryDate = request.RequestedDeliveryDate;
        order.PurchaseOrderReference = request.PurchaseOrderReference;
        order.SpecialInstructions = request.SpecialInstructions;
        order.InternalNotes = request.InternalNotes;
        order.IsPriorityOrder = request.IsPriorityOrder;
        order.UpdatedAt = DateTime.UtcNow;

        _orders.RemoveItems(order.OrderItems.ToList());
        order.OrderItems.Clear();

        foreach (var item in request.Items)
        {
            var gross = item.Quantity * item.UnitPrice;
            var discount = gross * (item.DiscountPercent / 100m);

            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                LineTotal = gross - discount,
                Notes = item.Notes
            });
        }

        order.Subtotal = order.OrderItems.Sum(x => x.Quantity * x.UnitPrice);
        order.DiscountAmount = order.OrderItems.Sum(x => x.Quantity * x.UnitPrice * x.DiscountPercent / 100m);
        order.TaxAmount = (order.Subtotal - order.DiscountAmount) * 0.2m;
        order.TotalAmount = order.Subtotal - order.DiscountAmount + order.TaxAmount;

        var newValues = CreateSnapshot(order);

        await _orders.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "Order",
            order.OrderId,
            "Updated",
            oldValues,
            newValues,
            $"Order updated: {order.OrderNumber}.",
            ct);
    }

    private static object CreateSnapshot(Order order)
    {
        return new
        {
            order.OrderId,
            order.OrderNumber,
            order.CustomerId,
            order.DeliveryAddressId,
            order.BillingAddressId,
            order.WarehouseId,
            order.CarrierId,
            order.ProjectId,
            order.RequestedDeliveryDate,
            order.PurchaseOrderReference,
            order.SpecialInstructions,
            order.InternalNotes,
            order.IsPriorityOrder,
            order.Subtotal,
            order.DiscountAmount,
            order.TaxAmount,
            order.TotalAmount,
            Items = order.OrderItems.Select(i => new
            {
                i.ProductId,
                i.Quantity,
                i.UnitPrice,
                i.DiscountPercent,
                i.LineTotal,
                i.Notes
            }).ToList()
        };
    }

    private static bool CanEditDraftOrder(IEnumerable<string> roles)
    {
        return roles.Any(role =>
            role.Equals("Sales", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase));
    }
}
