using Application.Features.Warehouses.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Warehouses.Queries.GetWarehouseById;

public class GetWarehouseByIdQueryHandler
    : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
{
    private readonly IWarehouseRepository _repo;

    public GetWarehouseByIdQueryHandler(IWarehouseRepository repo)
    {
        _repo = repo;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken ct)
    {
        var warehouse = await _repo.GetByIdAsync(request.WarehouseId, ct);

        if (warehouse == null)
            return null;

        return new WarehouseDto
        {
            WarehouseId = warehouse.WarehouseId,
            Name = warehouse.Name
        };
    }
}