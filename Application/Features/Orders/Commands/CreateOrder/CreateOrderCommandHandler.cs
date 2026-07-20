using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Validation;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Interfaces;
using Domain.Entities.Orders;
using Domain.Repositories;
using MediatR;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orders;
    private readonly IAuditService _audit;
    private readonly ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(
        IOrderRepository orders,
        IAuditService audit,
        ICurrentUserService currentUser)
    {
        _orders = orders;
        _audit = audit;
        _currentUser = currentUser;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand dto, CancellationToken ct)
    {
        ValidateRequest(dto);

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("User is not authenticated.");

        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CustomerId = dto.CustomerId,
            DeliveryAddressId = dto.DeliveryAddressId,
            BillingAddressId = dto.BillingAddressId,
            WarehouseId = dto.WarehouseId,
            CarrierId = dto.CarrierId,
            ProjectId = dto.ProjectId,
            CreatedByUserId = userId,
            RequestedDeliveryDate = dto.RequestedDeliveryDate,
            PurchaseOrderReference = dto.PurchaseOrderReference,
            SpecialInstructions = dto.SpecialInstructions,
            InternalNotes = dto.InternalNotes,
            IsPriorityOrder = dto.IsPriorityOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OrderStatusId = 1,
            Currency = "GBP"
        };

        foreach (var item in dto.Items)
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

        await _orders.AddAsync(order, ct);
        await _orders.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "Order",
            order.OrderId,
            "Created",
            null,
            new
            {
                order.OrderId,
                order.OrderNumber,
                order.CustomerId,
                order.OrderStatusId,
                order.TotalAmount,
                itemCount = order.OrderItems.Count
            },
            $"Order created: {order.OrderNumber}.",
            ct);

        return new CreateOrderResponse
        {
            OrderId = order.OrderId
        };
    }

    private static void ValidateRequest(CreateOrderCommand request)
    {
        CommandValidation.PositiveId(request.CustomerId, "Customer");
        CommandValidation.PositiveId(request.DeliveryAddressId, "Delivery address");
        CommandValidation.PositiveId(request.BillingAddressId, "Billing address");
        CommandValidation.PositiveId(request.WarehouseId, "Warehouse");
        CommandValidation.Date(request.RequestedDeliveryDate, "Requested delivery date");
        CommandValidation.OptionalText(request.PurchaseOrderReference, "Customer PO reference", 40);
        CommandValidation.OptionalText(request.SpecialInstructions, "Special instructions", 255);
        CommandValidation.OptionalText(request.InternalNotes, "Internal notes", 255);

        if (request.Items.Count == 0)
        {
            throw new BadRequestException("An order must have at least one order line.");
        }

        foreach (var item in request.Items)
        {
            ValidateOrderItem(item);
        }
    }

    private static void ValidateOrderItem(CreateOrderItemCommand item)
    {
        CommandValidation.PositiveId(item.ProductId, "Product");
        CommandValidation.Positive(item.Quantity, "Quantity");
        CommandValidation.NonNegative(item.UnitPrice, "Unit price");
        CommandValidation.Percentage(item.DiscountPercent, "Discount");
        CommandValidation.OptionalText(item.Notes, "Order line notes", 255);
    }
}
