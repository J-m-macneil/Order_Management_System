using Application.Features.Orders.Commands.ChangeOrderStatus;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.DiscardDraftOrder;
using Application.Features.Orders.Commands.UpdateOrder;
using Application.Features.Orders.Queries.GetAllowedStatuses;
using Application.Features.Orders.Queries.GetOrderById;
using Application.Features.Orders.Queries.GetOrders;
using Application.Features.Orders.Queries.GetOrderStatusHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "SalesOrAdmin")]
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "SalesOrAdmin")]
    public async Task<IActionResult> Update(int id, UpdateOrderCommand command, CancellationToken ct)
    {
        command.OrderId = id;
        await _mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "SalesOrAdmin")]
    public async Task<IActionResult> DiscardDraft(int id, CancellationToken ct)
    {
        await _mediator.Send(new DiscardDraftOrderCommand { OrderId = id }, ct);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery { OrderId = id });
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetOrdersQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("{id}/status")]
    [Authorize(Roles = "Sales,Operations,Admin")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeOrderStatusCommand command)
    {
        command.OrderId = id;
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet("{id}/history")]
    public async Task<IActionResult> GetHistory(int id)
    {
        var result = await _mediator.Send(new GetOrderStatusHistoryQuery { OrderId = id });
        return Ok(result);
    }

    [HttpGet("{id}/allowed-statuses")]
    [Authorize(Roles = "Sales,Operations,Admin")]
    public async Task<IActionResult> GetAllowedStatuses(int id)
    {
        var result = await _mediator.Send(new GetAllowedStatusesQuery { OrderId = id });
        return Ok(result);
    }
}
