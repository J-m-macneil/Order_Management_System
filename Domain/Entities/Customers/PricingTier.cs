namespace Domain.Entities.Customers;

public class PricingTier
{
    public int PricingTierId { get; set; }

    public string Name { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public bool PriorityProcessing { get; set; }
    public string? Description { get; set; }

    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}