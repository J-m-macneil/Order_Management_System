using Application.Features.Carriers.Queries.GetCarriers;
using Application.Features.Carriers.Queries.GetCarrierById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/carriers")]
[Authorize]
public class CarriersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CarriersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetCarriersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int carrierId)
    {
        var result = await _mediator.Send(new GetCarrierByIdQuery { CarrierId = carrierId });

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}