namespace Application.Features.Addresses.DTOs;

public class AddressDto
{
    public int AddressId { get; set; }
    public int? CustomerId { get; set; }

    public string AddressType { get; set; } = string.Empty;
    public string SiteName { get; set; } = string.Empty;

    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }

    public string City { get; set; } = string.Empty;
    public string? County { get; set; }
    public string Postcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string? ContactName { get; set; }
    public string? ContactPhone { get; set; }

    public string? DeliveryInstructions { get; set; }
}
