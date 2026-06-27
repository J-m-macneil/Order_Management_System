using MediatR;

namespace Application.Features.Orders.Commands.DiscardDraftOrder;

public class DiscardDraftOrderCommand : IRequest
{
    public int OrderId { get; set; }
}
