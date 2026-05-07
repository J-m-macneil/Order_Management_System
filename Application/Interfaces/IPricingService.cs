using Application.Features.Pricing.DTOs;

public interface IPricingService
{
    Task<PricingCalculationDto?> CalculatePriceAsync(int customerId, int productId);
}