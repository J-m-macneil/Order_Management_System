using Application.Features.Orders.Commands.ChangeOrderStatus;
using Application.Features.Orders.Commands.CreateOrder;
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
    public async Task<IActionResult> Create(CreateOrderCommand command)
    {
        var orderId = await _mediator.Send(command);
        return Ok(orderId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery { OrderId = id });
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetOrdersQuery());
        return Ok(result);
    }

    [HttpPost("{id}/status")]
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
    public async Task<IActionResult> GetAllowedStatuses(int id)
    {
        var result = await _mediator.Send(new GetAllowedStatusesQuery { OrderId = id });
        return Ok(result);
    }
}