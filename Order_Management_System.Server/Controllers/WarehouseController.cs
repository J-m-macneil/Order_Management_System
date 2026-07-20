using Application.Features.Warehouses.DTOs;
using Application.Features.Warehouses.Queries.GetWarehouseById;
using Application.Features.Warehouses.Queries.GetWarehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetWarehousesQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseDto>> GetById(int id)
    {
        var result = await _mediator.Send(new GetWarehouseByIdQuery
        {
            WarehouseId = id
        });

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}