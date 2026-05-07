using Application.Features.Orders.Commands.CreateOrder;
using Domain.Entities.Orders;
using Domain.Repositories;
using MediatR;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly IOrderRepository _orders;

    public CreateOrderCommandHandler(IOrderRepository orders)
    {
        _orders = orders;
    }

    public async Task<int> Handle(CreateOrderCommand dto, CancellationToken ct)
    {
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CustomerId = dto.CustomerId,
            DeliveryAddressId = dto.DeliveryAddressId,
            BillingAddressId = dto.BillingAddressId,
            WarehouseId = dto.WarehouseId,
            CarrierId = dto.CarrierId,
            CreatedByUserId = dto.CreatedByUserId,
            RequestedDeliveryDate = dto.RequestedDeliveryDate,
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

        return order.OrderId;
    }
}