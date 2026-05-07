namespace Application.Features.Carriers.DTOs;

public class CarrierDto
{
    public int CarrierId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? ContactEmail { get; set; }
    public string? ServiceType { get; set; }
}