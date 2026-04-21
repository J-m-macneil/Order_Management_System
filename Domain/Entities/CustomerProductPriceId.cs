namespace Domain.Entities;

public class CustomerProductPrice
{
    public int CustomerProductPriceId { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public decimal OverridePrice { get; set; }
    public decimal MinimumOrderQuantity { get; set; }

    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}