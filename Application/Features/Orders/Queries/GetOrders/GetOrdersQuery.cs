using Application.Features.Orders.DTOs;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQuery : IRequest<List<OrderDto>>
{
}