namespace Domain.Entities;

public class Customer
{
    public int CustomerId { get; set; }

    public string AccountNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string IndustryType { get; set; } = string.Empty;

    public string MainContactName { get; set; } = string.Empty;
    public string MainContactEmail { get; set; } = string.Empty;
    public string MainContactPhone { get; set; } = string.Empty;

    public int? BillingAddressId { get; set; }
    public Address? BillingAddress { get; set; }

    public int? DefaultDeliveryAddressId { get; set; }
    public Address? DefaultDeliveryAddress { get; set; }

    public int PricingTierId { get; set; }
    public PricingTier? PricingTier { get; set; }

    public int PaymentTermsDays { get; set; }
    public decimal CreditLimit { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}