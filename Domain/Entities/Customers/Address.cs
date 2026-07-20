using Domain.Entities.Orders;

namespace Domain.Entities.Customers;

public class Address
{
    public int AddressId { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string AddressType { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;

    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }

    public string City { get; set; } = string.Empty;
    public string? County { get; set; }
    public string Postcode { get; set; } = string.Empty;
    public string Country { get; set; } = "United Kingdom";

    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }

    public string? DeliveryInstructions { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Order> DeliveryOrders { get; set; } = new List<Order>();
    public ICollection<Order> BillingOrders { get; set; } = new List<Order>();
}
