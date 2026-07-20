using Domain.Entities.Orders;

public class OrderStatus
{
    public int OrderStatusId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsTerminal { get; set; }
    public int DisplayOrder { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}