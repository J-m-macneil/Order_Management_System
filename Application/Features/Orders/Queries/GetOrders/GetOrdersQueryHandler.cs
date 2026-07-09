using Application.Common.Models;
using Application.Features.Orders;
using Application.Features.Orders.DTOs;
using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler 
    : IRequestHandler<GetOrdersQuery, PagedResult<OrderDto>>
{
    private readonly IOrderRepository _repo;
    private readonly IOrderReviewPolicy _reviewPolicy;

    public GetOrdersQueryHandler(
        IOrderRepository repo,
        IOrderReviewPolicy reviewPolicy)
    {
        _repo = repo;
        _reviewPolicy = reviewPolicy;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken ct)
    {
        var totalCount = await _repo.CountActiveAsync(
            request.SearchTerm,
            request.OrderStatusId,
            request.IsPriorityOrder,
            request.HasRestrictedItems,
            request.RequestedDeliveryFrom,
            request.RequestedDeliveryTo,
            request.CreatedFrom,
            request.CreatedTo,
            ct);

        var orders = await _repo.GetPagedAsync(
            request.Skip,
            request.PageSize,
            request.SearchTerm,
            request.OrderStatusId,
            request.IsPriorityOrder,
            request.HasRestrictedItems,
            request.RequestedDeliveryFrom,
            request.RequestedDeliveryTo,
            request.CreatedFrom,
            request.CreatedTo,
            ct);

        var items = orders.Select(o =>
        {
            var effectiveStatus = OrderEffectiveStatus.From(o);
            var reviewReasons = _reviewPolicy.GetReviewReasons(o);

            return new OrderDto
            {
                OrderId = o.OrderId,
                OrderNumber = o.OrderNumber,

                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.CompanyName,

                OrderStatusId = effectiveStatus.StatusId,
                OrderStatusName = effectiveStatus.StatusName,

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

                IsPriorityOrder = o.IsPriorityOrder,
                HasRestrictedItems = reviewReasons.Count > 0,
                ReviewReasons = reviewReasons.ToList(),
                FailedProcessingJobCount = effectiveStatus.FailedProcessingJobCount
            };
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


