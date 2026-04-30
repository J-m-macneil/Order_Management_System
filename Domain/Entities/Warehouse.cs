using Domain.Entities.Orders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Warehouse
{
    public int WarehouseId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public int AddressId { get; set; }
    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }

    public bool IsActive { get; set; }

    public Address Address { get; set; } = null!;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}