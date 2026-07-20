using Application.Features.Warehouses.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Warehouses.Queries.GetWarehouses;

public class GetWarehousesQueryHandler
    : IRequestHandler<GetWarehousesQuery, List<WarehouseDto>>
{
    private readonly IWarehouseRepository _repo;

    public GetWarehousesQueryHandler(IWarehouseRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<WarehouseDto>> Handle(GetWarehousesQuery request, CancellationToken ct)
    {
        var warehouses = await _repo.GetAllAsync(ct);

        return warehouses
            .OrderBy(x => x.Name)
            .Select(x => new WarehouseDto
            {
                WarehouseId = x.WarehouseId,
                Name = x.Name
            })
            .ToList();
    }
}