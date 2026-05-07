using Application.Features.Warehouses.DTOs;
using MediatR;

namespace Application.Features.Warehouses.Queries.GetWarehouses;

public class GetWarehousesQuery : IRequest<List<WarehouseDto>>
{
}