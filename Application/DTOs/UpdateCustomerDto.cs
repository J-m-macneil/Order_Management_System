using Domain.Entities;

namespace Application.DTOs;

public class UpdateCustomerDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string IndustryType { get; set; } = string.Empty;
    public string MainContactName { get; set; } = string.Empty;
    public string MainContactEmail { get; set; } = string.Empty;
    public string MainContactPhone { get; set; } = string.Empty;
    public int? BillingAddressId { get; set; }
    public int? DefaultDeliveryAddressId { get; set; }
    public int PricingTierId { get; set; }
    public int PaymentTermsDays { get; set; }
    public decimal CreditLimit { get; set; }
    public bool IsActive { get; set; }
}