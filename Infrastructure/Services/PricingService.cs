using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

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

        if (customer == null || product == null || customer.PricingTier == null)
            return null;

        var discountPercent = customer.PricingTier.DiscountPercent;
        var basePrice = product.BasePrice;
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
            Currency = product.Currency
        };
    }
}