namespace Application.DTOs;

public class CreateOrderItemDto
{
    public int ProductId { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }

    public string? Notes { get; set; }
}