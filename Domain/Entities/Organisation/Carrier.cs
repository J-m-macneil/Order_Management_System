using Domain.Entities.Orders;

namespace Domain.Entities.Organisation;

public class Carrier
{
    public int CarrierId { get; set; }
    public string Name { get; set; } = null!;
    public string? ContactEmail { get; set; }
    public string? ServiceType { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}