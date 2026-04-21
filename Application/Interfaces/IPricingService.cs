using Application.DTOs;

public interface IPricingService
{
    Task<PricingCalculationDto?> CalculatePriceAsync(int customerId, int productId);
}