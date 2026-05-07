using Application.Features.Orders.DTOs;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllowedStatuses;

public class GetAllowedStatusesQuery : IRequest<List<AllowedStatusDto>>
{
    public int OrderId { get; set; }
}