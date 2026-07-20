namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderItemCommand
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public string? Notes { get; set; }
}