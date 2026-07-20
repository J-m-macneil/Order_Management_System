using Application.Features.Dashboard.Queries.GetDashboardMetrics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin,Operations,Sales,Demo")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardMetricsQuery(), ct);
        return Ok(result);
    }
}
