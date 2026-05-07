using Application.Features.Warehouses.DTOs;
using MediatR;

namespace Application.Features.Warehouses.Queries.GetWarehouseById;

public class GetWarehouseByIdQuery : IRequest<WarehouseDto?>
{
    public int WarehouseId { get; set; }
}