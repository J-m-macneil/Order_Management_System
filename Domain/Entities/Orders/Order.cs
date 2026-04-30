using Domain.Entities.Customers;

namespace Domain.Entities.Orders;

public class Order
{
    public int OrderId { get; set; }

    public string OrderNumber { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int? ProjectId { get; set; }
    public Project? Project { get; set; }

    public int DeliveryAddressId { get; set; }
    public Address DeliveryAddress { get; set; } = null!;

    public int BillingAddressId { get; set; }
    public Address BillingAddress { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = null!;

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }

    public int OrderStatusId { get; set; } = 1;
    public OrderStatus OrderStatus { get; set; } = null!;

    public DateTime RequestedDeliveryDate { get; set; }
    public DateTime? SubmittedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public string Currency { get; set; } = "GBP";

    public decimal Subtotal { get; set; } = 0m;
    public decimal DiscountAmount { get; set; } = 0m;
    public decimal TaxAmount { get; set; } = 0m;
    public decimal TotalAmount { get; set; } = 0m;

    public string? PurchaseOrderReference { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? InternalNotes { get; set; }
    public string? FailureReason { get; set; }

    public bool IsPriorityOrder { get; set; } = false;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> OrderStatusHistory { get; set; } = new List<OrderStatusHistory>();
}