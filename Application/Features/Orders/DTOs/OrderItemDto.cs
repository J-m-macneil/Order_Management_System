namespace Application.Features.Orders.DTOs;

public class OrderItemDto
{
    public int OrderItemId { get; set; }

    public int ProductId { get; set; }
    public string? ProductName { get; set; } 
    public string? ProductSku { get; set; }
    public string? PackSize { get; set; }
    public string? UNNumber { get; set; }
    public bool RequiresSds { get; set; }
    public bool IsRestricted { get; set; }

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal LineTotal { get; set; }

    public string? Notes { get; set; }
}
