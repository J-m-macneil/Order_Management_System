using Application.Common.Exceptions;
using Application.Features.Orders.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderStatusHistory;

public class GetOrderStatusHistoryQueryHandler
    : IRequestHandler<GetOrderStatusHistoryQuery, List<OrderStatusHistoryDto>>
{
    private readonly IOrderRepository _repo;

    public GetOrderStatusHistoryQueryHandler(IOrderRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<OrderStatusHistoryDto>> Handle(
        GetOrderStatusHistoryQuery request,
        CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        return order.OrderStatusHistory
            .OrderByDescending(x => x.ChangedAt)
            .Select(h => new OrderStatusHistoryDto
            {
                OrderStatusHistoryId = h.OrderStatusHistoryId,
                FromStatusName = h.FromStatus?.Name,
                ToStatusName = h.ToStatus.Name,
                ChangedByUserName = h.ChangedByUser.FullName,
                ChangedAt = h.ChangedAt,
                Reason = h.Reason
            })
            .ToList();
    }
}
