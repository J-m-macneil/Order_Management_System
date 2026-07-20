public class ProductListDto
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public string ProductCategoryName { get; set; } = string.Empty;
    public string UnitOfMeasureName { get; set; } = string.Empty;
    public string HazardClassName { get; set; } = string.Empty;

    public string PackSize { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "GBP";

    public bool IsRestricted { get; set; }
    public bool IsActive { get; set; }
}