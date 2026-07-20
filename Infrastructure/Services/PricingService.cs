using Application.Features.Pricing.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Pricing.Queries;

public class PricingService : IPricingService
{
    private readonly AppDbContext _dbContext;

    public PricingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PricingTierDto>> GetTiersAsync()
    {
        return await _dbContext.PricingTiers
            .OrderBy(x => x.PriorityProcessing)
            .Select(x => new PricingTierDto
            {
                PricingTierId = x.PricingTierId,
                Name = x.Name,
                DiscountPercent = x.DiscountPercent,
                PriorityProcessing = x.PriorityProcessing,
                Description = x.Description
            })
            .ToListAsync();
    }

    public async Task<PricingCalculationDto?> CalculatePriceAsync(int customerId, int productId)
    {
        var breakdown = await GetBreakdownAsync(customerId, productId);

        if (breakdown == null)
            return null;

        return new PricingCalculationDto
        {
            CustomerId = customerId,
            ProductId = productId,
            BasePrice = breakdown.BasePrice,
            DiscountPercent = breakdown.TierDiscountPercent,
            DiscountAmount = breakdown.TierDiscountAmount,
            FinalPrice = breakdown.FinalPrice,
            PricingTierName = breakdown.PricingTierName,
            Currency = "GBP", // adjust if product supports multi-currency
            IsOverrideApplied = breakdown.OverridePrice.HasValue
        };
    }

    public async Task<PricingBreakdownDto?> GetBreakdownAsync(int customerId, int productId)
    {
        var customer = await _dbContext.Customers
            .Include(x => x.PricingTier)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.DeletedAt == null);

        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.DeletedAt == null);

        if (customer == null || customer.PricingTier == null || product == null)
            return null;

        var basePrice = product.BasePrice;

        var tier = customer.PricingTier;

        // =========================
        // TIER CALCULATION
        // =========================
        var tierDiscountPercent = tier.DiscountPercent;
        var tierDiscountAmount = Math.Round(basePrice * (tierDiscountPercent / 100m), 2);
        var priceAfterTier = Math.Round(basePrice - tierDiscountAmount, 2);

        // =========================
        // OVERRIDE CHECK (Contract pricing)
        // =========================
        var today = DateTime.UtcNow.Date;

        var overridePrice = await _dbContext.CustomerProductPrices
            .Where(x =>
                x.CustomerId == customerId &&
                x.ProductId == productId &&
                x.IsActive &&
                x.DeletedAt == null &&
                x.EffectiveFrom <= today &&
                (x.EffectiveTo == null || x.EffectiveTo >= today))
            .OrderByDescending(x => x.EffectiveFrom)
            .FirstOrDefaultAsync();

        decimal finalPrice;
        string source;

        if (overridePrice != null)
        {
            finalPrice = overridePrice.OverridePrice;
            source = "Contract Override";
        }
        else
        {
            finalPrice = priceAfterTier;
            source = "Tier Pricing";
        }

        return new PricingBreakdownDto
        {
            BasePrice = basePrice,
            PricingTierName = tier.Name,
            TierDiscountPercent = tierDiscountPercent,
            TierDiscountAmount = tierDiscountAmount,
            PriceAfterTier = priceAfterTier,
            OverridePrice = overridePrice?.OverridePrice,
            FinalPrice = finalPrice,
            PricingSource = source,
            Notes = overridePrice != null
                ? "Customer-specific contract pricing applied"
                : $"Standard tier pricing applied ({tier.Name})"
        };
    }
}