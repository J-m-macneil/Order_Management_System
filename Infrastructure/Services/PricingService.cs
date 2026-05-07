using Application.Features.Pricing.DTOs;
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

    public async Task<PricingCalculationDto?> CalculatePriceAsync(int customerId, int productId)
    {
        var customer = await _dbContext.Customers
            .Include(x => x.PricingTier)
            .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.DeletedAt == null);

        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.DeletedAt == null);

        if (customer == null || customer.PricingTier == null || product == null)
            return null;

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

        if (overridePrice != null)
        {
            return new PricingCalculationDto
            {
                CustomerId = customerId,
                ProductId = productId,
                BasePrice = product.BasePrice,
                DiscountPercent = 0,
                DiscountAmount = 0,
                FinalPrice = overridePrice.OverridePrice,
                PricingTierName = customer.PricingTier.Name,
                Currency = product.Currency,
                IsOverrideApplied = true
            };
        }

        var basePrice = product.BasePrice;
        var discountPercent = customer.PricingTier.DiscountPercent;
        var discountAmount = Math.Round(basePrice * (discountPercent / 100m), 2);
        var finalPrice = Math.Round(basePrice - discountAmount, 2);

        return new PricingCalculationDto
        {
            CustomerId = customerId,
            ProductId = productId,
            BasePrice = basePrice,
            DiscountPercent = discountPercent,
            DiscountAmount = discountAmount,
            FinalPrice = finalPrice,
            PricingTierName = customer.PricingTier.Name,
            Currency = product.Currency,
            IsOverrideApplied = false
        };
    }
}