using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int ProductCategoryId { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string PackSize { get; set; } = string.Empty;

        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = "GBP";

        public int HazardClassId { get; set; }
        public string? UNNumber { get; set; }
        public string? StorageRequirement { get; set; }

        public bool RequiresSds { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsActive { get; set; }
    }
}
