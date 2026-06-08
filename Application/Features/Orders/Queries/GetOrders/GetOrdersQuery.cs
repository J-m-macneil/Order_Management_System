using Application.Common.Models;
using Application.Features.Orders.DTOs;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrders;

public class GetOrdersQuery : PaginationQuery, IRequest<PagedResult<OrderDto>>
{
    public string? SearchTerm { get; set; }
    public int? OrderStatusId { get; set; }
    public bool? IsPriorityOrder { get; set; }
    public DateTime? RequestedDeliveryFrom { get; set; }
    public DateTime? RequestedDeliveryTo { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
