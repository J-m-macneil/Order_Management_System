using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Pricing.DTOs;

public class PricingBreakdownDto
{
    // Base product price before any customer logic
    public decimal BasePrice { get; set; }

    // Pricing tier applied (e.g. Gold = 7.5%)
    public string PricingTierName { get; set; } = string.Empty;

    public decimal TierDiscountPercent { get; set; }

    public decimal TierDiscountAmount { get; set; }

    // Final price after tier discount but before overrides
    public decimal PriceAfterTier { get; set; }

    // Contract / customer-specific override (if any)
    public decimal? OverridePrice { get; set; }

    // Final resolved price used in order
    public decimal FinalPrice { get; set; }

    // Where the price came from (important for audit/UI)
    public string PricingSource { get; set; } = string.Empty;

    // Debug / explainability field (optional but powerful)
    public string? Notes { get; set; }
}