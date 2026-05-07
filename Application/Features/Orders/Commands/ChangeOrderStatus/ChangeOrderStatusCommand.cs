using MediatR;

namespace Application.Features.Orders.Commands.ChangeOrderStatus;

public class ChangeOrderStatusCommand : IRequest<bool>
{
    public int OrderId { get; set; }
    public int StatusId { get; set; }
    public string? Reason { get; set; }
}