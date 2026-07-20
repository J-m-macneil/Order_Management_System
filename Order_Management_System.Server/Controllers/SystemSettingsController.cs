using Application.Features.SystemSettings.Commands.UpdateSystemSetting;
using Application.Features.SystemSettings.Queries.GetSystemSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/system-settings")]
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SystemSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Demo")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSystemSettingsQuery(), ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, UpdateSystemSettingCommand command, CancellationToken ct)
    {
        await _mediator.Send(new UpdateSystemSettingRequest
        {
            SystemSettingId = id,
            Data = command
        }, ct);

        return NoContent();
    }
}
