using Application.Features.Pricing.DTOs;

namespace Application.Interfaces;

public interface IPricingService
{
    Task<List<PricingTierDto>> GetTiersAsync();

    Task<PricingCalculationDto?> CalculatePriceAsync(int customerId, int productId);

    Task<PricingBreakdownDto?> GetBreakdownAsync(int customerId, int productId);
}