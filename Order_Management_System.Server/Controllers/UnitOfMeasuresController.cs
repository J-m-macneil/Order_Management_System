using Application.Features.Products.DTOs;
using Application.Features.Products.Queries.GetUnitOfMeasures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/unit-of-measures")]
[Authorize]
public class UnitOfMeasuresController : ControllerBase
{
    private readonly IMediator _mediator;

    public UnitOfMeasuresController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitOfMeasureDto>>> Get()
    {
        var result = await _mediator.Send(new GetUnitOfMeasuresQuery());
        return Ok(result);
    }
}