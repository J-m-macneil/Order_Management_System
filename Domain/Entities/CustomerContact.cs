namespace Domain.Entities;

public class CustomerContact
{
    public int CustomerContactId { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }

    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}