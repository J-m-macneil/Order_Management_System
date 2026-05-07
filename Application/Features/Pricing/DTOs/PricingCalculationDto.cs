namespace Application.Features.Pricing.DTOs;

public class PricingCalculationDto
{
    public int CustomerId { get; set; }
    public int ProductId { get; set; }

    public decimal BasePrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }

    public string PricingTierName { get; set; } = string.Empty;
    public string Currency { get; set; } = "GBP";

    public bool IsOverrideApplied { get; set; }
    public decimal? OverridePrice { get; set; }
    public decimal? MinimumOrderQuantity { get; set; }
}