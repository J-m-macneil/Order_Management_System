using Domain.Entities.Customers;
using Domain.Entities.Orders;
using Domain.Entities.Products;

namespace Domain.Entities;

public class Product
{
    public int ProductId { get; set; }

    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int ProductCategoryId { get; set; }
    public ProductCategory ProductCategory { get; set; } = null!;
    public int UnitOfMeasureId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = null!;
    public string PackSize { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "GBP";

    public int HazardClassId { get; set; }
    public HazardClass HazardClass { get; set; } = null!;
    public string? UNNumber { get; set; }
    public string? StorageRequirement { get; set; }

    public bool RequiresSds { get; set; }
    public bool IsRestricted { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
    public ICollection<SafetyDataSheet> SafetyDataSheets { get; set; } = new List<SafetyDataSheet>();
    public ICollection<CustomerProductPrice> CustomerProductPrices { get; set; } = new List<CustomerProductPrice>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}