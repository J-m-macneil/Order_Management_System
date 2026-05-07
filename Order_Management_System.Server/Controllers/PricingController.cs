using Application.Features.Pricing.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/pricing")]
[Authorize]
public class PricingController : ControllerBase
{
    private readonly IPricingService _pricingService;

    public PricingController(IPricingService pricingService)
    {
        _pricingService = pricingService;
    }

    [HttpGet("calculate")]
    public async Task<ActionResult<PricingCalculationDto>> Calculate([FromQuery] int customerId, [FromQuery] int productId)
    {
        var result = await _pricingService.CalculatePriceAsync(customerId, productId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}