using Application.Features.Users.Commands.CreateUser;
using Application.Features.Users.Commands.UpdateUser;
using Application.Features.Users.Queries.GetDepartments;
using Application.Features.Users.Queries.GetRoles;
using Application.Features.Users.Queries.GetUserById;
using Application.Features.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Demo")]
    public async Task<IActionResult> Get([FromQuery] GetUsersQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Demo")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { UserId = id }, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("roles")]
    [Authorize(Roles = "Admin,Demo")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRolesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("departments")]
    [Authorize(Roles = "Admin,Demo")]
    public async Task<IActionResult> GetDepartments(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDepartmentsQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Create(CreateUserCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(int id, UpdateUserCommand command, CancellationToken ct)
    {
        await _mediator.Send(new UpdateUserRequest
        {
            UserId = id,
            Data = command
        }, ct);

        return NoContent();
    }
}
