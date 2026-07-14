using Application.Features.Addresses.DTOs;

namespace Application.Features.Orders.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public int DeliveryAddressId { get; set; }
    public int BillingAddressId { get; set; }
    public AddressDto? DeliveryAddress { get; set; }
    public AddressDto? BillingAddress { get; set; }

    public int OrderStatusId { get; set; }

    public string? CustomerName { get; set; } 

    public int WarehouseId { get; set; }
    public string? WarehouseName { get; set; }

    public int? CarrierId { get; set; }
    public string? CarrierName { get; set; }

    public int? ProjectId { get; set; }
    public string? ProjectName { get; set; }

    public string? OrderStatusName { get; set; }

    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }

    public DateTime RequestedDeliveryDate { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string Currency { get; set; } = "GBP";

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string? PurchaseOrderReference { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? InternalNotes { get; set; }
    public string? FailureReason { get; set; }

    public bool IsPriorityOrder { get; set; }
    public bool HasRestrictedItems { get; set; }
    public bool RequiresSdsBundle { get; set; }
    public List<string> ReviewReasons { get; set; } = new();
    public int FailedProcessingJobCount { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}
