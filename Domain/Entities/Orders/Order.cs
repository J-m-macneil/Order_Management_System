using Domain.Entities.Customers;
using Domain.Entities.Identity;
using Domain.Entities.Organisation;
using Domain.Entities.Status;
using Domain.Enums;
using Domain.Rules;

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
    public ICollection<ProcessingJob> ProcessingJobs { get; set; } = new List<ProcessingJob>();

    public IReadOnlyCollection<Product> GetProductsRequiringSafetyDataSheets()
    {
        return OrderItems
            .Where(item => item.DeletedAt == null && item.Product.RequiresSds)
            .Select(item => item.Product)
            .DistinctBy(product => product.ProductId)
            .ToList();
    }

    public void DiscardDraft()
    {
        if (DeletedAt != null)
            return;

        if ((OrderStatusEnum)OrderStatusId != OrderStatusEnum.Draft)
            throw new InvalidOperationException("Only draft orders can be discarded.");

        var discardedAt = DateTime.UtcNow;

        DeletedAt = discardedAt;
        UpdatedAt = discardedAt;

        foreach (var item in OrderItems.Where(x => x.DeletedAt == null))
        {
            item.DeletedAt = discardedAt;
        }
    }

    public void ChangeStatus(int newStatusId, int userId, string? reason)
    {
        if (OrderStatusId == newStatusId)
            return;

        var fromStatus = (OrderStatusEnum)OrderStatusId;
        var toStatus = (OrderStatusEnum)newStatusId;

        if (!OrderStatusTransitions.CanTransition(fromStatus, toStatus))
            throw new InvalidOperationException(
                $"Invalid transition from {fromStatus} to {toStatus}");

        OrderStatusHistory.Add(new OrderStatusHistory
        {
            OrderId = OrderId,
            FromStatusId = OrderStatusId,
            ToStatusId = newStatusId,
            ChangedByUserId = userId,
            Reason = reason,
            ChangedAt = DateTime.UtcNow
        });

        OrderStatusId = newStatusId;
        UpdatedAt = DateTime.UtcNow;

        if (toStatus is OrderStatusEnum.Cancelled or OrderStatusEnum.Failed)
            FailureReason = reason;
    }
}
