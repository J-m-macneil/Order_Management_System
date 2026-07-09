using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Orders.DTOs;
using Domain.Enums;
using Domain.Repositories;
using Domain.Rules;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllowedStatuses;

public class GetAllowedStatusesQueryHandler
    : IRequestHandler<GetAllowedStatusesQuery, List<AllowedStatusDto>>
{
    private readonly IOrderRepository _repo;
    private readonly ICurrentUserService _currentUser;

    public GetAllowedStatusesQueryHandler(
        IOrderRepository repo,
        ICurrentUserService currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<List<AllowedStatusDto>> Handle(
        GetAllowedStatusesQuery request,
        CancellationToken ct)
    {
        var order = await _repo.GetByIdAsync(request.OrderId, ct);

        if (order == null)
            throw new NotFoundException("Order", request.OrderId);

        var roles = _currentUser.Roles;

        var currentStatus = (OrderStatusEnum)order.OrderStatusId;

        var allowedStatusEnums = OrderStatusTransitions
            .GetAllowed(currentStatus, roles)
            .ToList();

        var statuses = await _repo.GetAllStatusesAsync(ct);

        return statuses
            .Where(s => allowedStatusEnums.Contains((OrderStatusEnum)s.OrderStatusId))
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new AllowedStatusDto
            {
                Id = s.OrderStatusId,
                Name = s.Name
            })
            .ToList();
    }
}
