using Application.Features.Orders.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _repo;

    public GetOrderByIdQueryHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new KeyNotFoundException($"Order {request.OrderId} not found");

        return new OrderDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,

            CustomerId = order.CustomerId,
            CustomerName = order.Customer?.CompanyName,

            DeliveryAddressId = order.DeliveryAddressId,
            BillingAddressId = order.BillingAddressId,

            WarehouseId = order.WarehouseId,
            WarehouseName = order.Warehouse?.Name,

            CarrierId = order.CarrierId,
            CarrierName = order.Carrier?.Name,

            ProjectId = order.ProjectId,
            ProjectName = order.Project?.ProjectName,

            OrderStatusId = order.OrderStatusId,
            OrderStatusName = order.OrderStatus?.Name,

            CreatedByUserId = order.CreatedByUserId,
            AssignedToUserId = order.AssignedToUserId,

            RequestedDeliveryDate = order.RequestedDeliveryDate,
            SubmittedAt = order.SubmittedAt,

            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,

            Currency = order.Currency,

            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,

            PurchaseOrderReference = order.PurchaseOrderReference,
            SpecialInstructions = order.SpecialInstructions,
            InternalNotes = order.InternalNotes,
            FailureReason = order.FailureReason,

            IsPriorityOrder = order.IsPriorityOrder,

            Items = order.OrderItems?.Select(i => new OrderItemDto
            {
                OrderItemId = i.OrderItemId,
                ProductId = i.ProductId,
                ProductName = i.Product?.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountPercent = i.DiscountPercent,
                LineTotal = i.LineTotal,
                Notes = i.Notes
            }).ToList() ?? new List<OrderItemDto>()
        };
    }
}