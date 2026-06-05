using Application.Common.Models;
using Application.Features.Orders.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler 
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _repo;

    public GetOrdersQueryHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var totalCount = await _repo.CountActiveAsync(ct);

        var orders = await _repo.GetPagedAsync(
         request.Skip,
         request.PageSize,
         ct);

        var items = orders.Select(o => new OrderDto
        {
            OrderId = o.OrderId,
            OrderNumber = o.OrderNumber,

            CustomerId = o.CustomerId,
            CustomerName = o.Customer?.CompanyName,

            OrderStatusId = o.OrderStatusId,
            OrderStatusName = o.OrderStatus?.Name,

            WarehouseId = o.WarehouseId,
            WarehouseName = o.Warehouse?.Name,

            CarrierId = o.CarrierId,
            CarrierName = o.Carrier?.Name,

            ProjectId = o.ProjectId,

            CreatedByUserId = o.CreatedByUserId,
            AssignedToUserId = o.AssignedToUserId,

            RequestedDeliveryDate = o.RequestedDeliveryDate,
            SubmittedAt = o.SubmittedAt,

            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt,

            Currency = o.Currency,

            Subtotal = o.Subtotal,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            TotalAmount = o.TotalAmount,

            PurchaseOrderReference = o.PurchaseOrderReference,
            SpecialInstructions = o.SpecialInstructions,
            InternalNotes = o.InternalNotes,
            FailureReason = o.FailureReason,

            IsPriorityOrder = o.IsPriorityOrder
        }).ToList();

        return new PagedResult<OrderDto>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        };
    }
}