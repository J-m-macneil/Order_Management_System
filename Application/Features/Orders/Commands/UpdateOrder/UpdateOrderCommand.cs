using Application.Features.Orders.Commands.CreateOrder;
using MediatR;

namespace Application.Features.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int DeliveryAddressId { get; set; }
    public int BillingAddressId { get; set; }
    public int WarehouseId { get; set; }
    public int? CarrierId { get; set; }
    public int? ProjectId { get; set; }
    public DateTime RequestedDeliveryDate { get; set; }
    public string? PurchaseOrderReference { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? InternalNotes { get; set; }
    public bool IsPriorityOrder { get; set; }

    public List<CreateOrderItemCommand> Items { get; set; } = new();
}
