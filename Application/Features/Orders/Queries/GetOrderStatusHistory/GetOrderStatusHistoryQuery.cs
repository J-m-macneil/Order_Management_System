using Application.Features.Orders.DTOs;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderStatusHistory;

public class GetOrderStatusHistoryQuery : IRequest<List<OrderStatusHistoryDto>>
{
    public int OrderId { get; set; }
}