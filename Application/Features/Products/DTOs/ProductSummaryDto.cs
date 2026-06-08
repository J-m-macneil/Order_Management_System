namespace Application.Features.Products.DTOs;

public class ProductSummaryDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int RestrictedProducts { get; set; }
    public int HazardousProducts { get; set; }
}
