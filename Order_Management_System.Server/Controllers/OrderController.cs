using Application.DTOs;
using Domain.Entities.Orders;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IProcessingJobQueueService _processingJobQueueService;

    public OrdersController(
        AppDbContext dbContext,
        IProcessingJobQueueService processingJobQueueService)
    {
        _dbContext = dbContext;
        _processingJobQueueService = processingJobQueueService;
    }

    // =========================
    // CREATE ORDER
    // =========================
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            return BadRequest("Order must contain at least one item.");

        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
            CustomerId = dto.CustomerId,
            DeliveryAddressId = dto.DeliveryAddressId,
            BillingAddressId = dto.BillingAddressId,
            CreatedByUserId = dto.CreatedByUserId,
            WarehouseId = dto.WarehouseId,
            CarrierId = dto.CarrierId,
            OrderStatusId = 1,
            RequestedDeliveryDate = dto.RequestedDeliveryDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Currency = "GBP",
            IsPriorityOrder = dto.IsPriorityOrder
        };

        // =========================
        // ADD ITEMS
        // =========================
        foreach (var item in dto.Items)
        {
            var gross = item.Quantity * item.UnitPrice;
            var discountAmount = gross * (item.DiscountPercent / 100m);
            var lineTotal = Math.Round(gross - discountAmount, 2);

            order.OrderItems.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                LineTotal = lineTotal,
                Notes = item.Notes
            });
        }

        // =========================
        // CALCULATE TOTALS
        // =========================
        order.Subtotal = order.OrderItems.Sum(x => x.Quantity * x.UnitPrice);

        order.DiscountAmount = order.OrderItems.Sum(x =>
            (x.Quantity * x.UnitPrice) * (x.DiscountPercent / 100m)
        );

        order.TaxAmount = Math.Round((order.Subtotal - order.DiscountAmount) * 0.2m, 2); // 20% VAT

        order.TotalAmount = order.Subtotal - order.DiscountAmount + order.TaxAmount;

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        // =========================
        // RETURN DTO
        // =========================
        var result = new OrderDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,

            CustomerId = order.CustomerId,
            DeliveryAddressId = order.DeliveryAddressId,
            BillingAddressId = order.BillingAddressId,

            CreatedByUserId = order.CreatedByUserId,
            AssignedToUserId = order.AssignedToUserId,

            RequestedDeliveryDate = order.RequestedDeliveryDate,
            SubmittedAt = order.SubmittedAt,

            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,

            Currency = order.Currency,

            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,

            PurchaseOrderReference = order.PurchaseOrderReference,
            SpecialInstructions = order.SpecialInstructions,
            InternalNotes = order.InternalNotes,
            FailureReason = order.FailureReason,

            IsPriorityOrder = order.IsPriorityOrder,

            Items = order.OrderItems.Select(x => new OrderItemDto
            {
                OrderItemId = x.OrderItemId,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                DiscountPercent = x.DiscountPercent,
                LineTotal = x.LineTotal,
                Notes = x.Notes
            }).ToList()
        };

        return Ok(result);
    }

    // =========================
    // GET ALL ORDERS
    // =========================
    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll()
    {
        var orders = await _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Warehouse)
            .Include(o => o.Carrier)
            .Include(o => o.Project)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.DeletedAt == null)
            .ToListAsync();

        var result = orders.Select(order => new OrderDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,

            CustomerId = order.CustomerId,
            CustomerName = order.Customer != null ? order.Customer.CompanyName : null,

            DeliveryAddressId = order.DeliveryAddressId,
            BillingAddressId = order.BillingAddressId,

            WarehouseId = order.WarehouseId,
            WarehouseName = order.Warehouse != null ? order.Warehouse.Name : null,

            CarrierId = order.CarrierId,
            CarrierName = order.Carrier != null ? order.Carrier.Name : null,

            ProjectId = order.ProjectId,
            ProjectName = order.Project != null ? order.Project.ProjectName : null,

            OrderStatusId = order.OrderStatusId,
            OrderStatusName = order.OrderStatus != null ? order.OrderStatus.Name : null,

            CreatedByUserId = order.CreatedByUserId,
            AssignedToUserId = order.AssignedToUserId,

            RequestedDeliveryDate = order.RequestedDeliveryDate,
            SubmittedAt = order.SubmittedAt,

            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,

            Currency = order.Currency,

            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,

            PurchaseOrderReference = order.PurchaseOrderReference,
            SpecialInstructions = order.SpecialInstructions,
            InternalNotes = order.InternalNotes,
            FailureReason = order.FailureReason,

            IsPriorityOrder = order.IsPriorityOrder,

            Items = order.OrderItems.Select(x => new OrderItemDto
            {
                OrderItemId = x.OrderItemId,
                ProductId = x.ProductId,
                ProductName = x.Product != null ? x.Product.ProductName : null,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                DiscountPercent = x.DiscountPercent,
                LineTotal = x.LineTotal,
                Notes = x.Notes
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    // =========================
    // GET ORDER BY ID
    // =========================
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Warehouse)
            .Include(o => o.Carrier)
            .Include(o => o.Project)
            .Include(o => o.OrderStatus)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.DeletedAt == null);

        if (order == null)
            return NotFound();

        var result = new OrderDto
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,

            CustomerId = order.CustomerId,
            CustomerName = order.Customer != null ? order.Customer.CompanyName : null,

            DeliveryAddressId = order.DeliveryAddressId,
            BillingAddressId = order.BillingAddressId,

            WarehouseId = order.WarehouseId,
            WarehouseName = order.Warehouse != null ? order.Warehouse.Name : null,

            CarrierId = order.CarrierId,
            CarrierName = order.Carrier != null ? order.Carrier.Name : null,

            ProjectId = order.ProjectId,
            ProjectName = order.Project != null ? order.Project.ProjectName : null,

            OrderStatusId = order.OrderStatusId,
            OrderStatusName = order.OrderStatus != null ? order.OrderStatus.Name : null,

            CreatedByUserId = order.CreatedByUserId,
            AssignedToUserId = order.AssignedToUserId,

            RequestedDeliveryDate = order.RequestedDeliveryDate,
            SubmittedAt = order.SubmittedAt,

            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,

            Currency = order.Currency,

            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            TotalAmount = order.TotalAmount,

            PurchaseOrderReference = order.PurchaseOrderReference,
            SpecialInstructions = order.SpecialInstructions,
            InternalNotes = order.InternalNotes,
            FailureReason = order.FailureReason,

            IsPriorityOrder = order.IsPriorityOrder,

            Items = order.OrderItems.Select(x => new OrderItemDto
            {
                OrderItemId = x.OrderItemId,
                ProductId = x.ProductId,
                ProductName = x.Product != null ? x.Product.ProductName : null,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                DiscountPercent = x.DiscountPercent,
                LineTotal = x.LineTotal,
                Notes = x.Notes
            }).ToList()
        };

        return Ok(result);
    }

    [HttpGet("{id:int}/allowed-statuses")]
    [Authorize(Roles = "Sales,Operations,Admin")]
    public async Task<IActionResult> GetAllowedStatuses(int id)
    {
        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.OrderId == id && o.DeletedAt == null);

        if (order == null)
            return NotFound();

        var roles = User.Claims
            .Where(c =>
                c.Type == System.Security.Claims.ClaimTypes.Role ||
                c.Type == "role")
            .Select(c => c.Value)
            .ToList();

        if (!roles.Any())
            return Forbid();

        var currentStatus = (OrderStatusEnum)order.OrderStatusId;

        var allowedStatusIds = new List<int>();

        foreach (var role in roles)
        {
            var allowed = OrderStatusTransitions.GetAllowedTransitions(
                currentStatus,
                role
            );

            allowedStatusIds.AddRange(allowed.Select(x => (int)x));
        }

        allowedStatusIds = allowedStatusIds
            .Distinct()
            .ToList();

        var result = await _dbContext.OrderStatuses
            .Where(s => allowedStatusIds.Contains(s.OrderStatusId))
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new AllowedStatusDto
            {
                Id = s.OrderStatusId,
                Name = s.Name
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpPost("{id:int}/status")]
    [Authorize(Roles = "Sales,Operations,Admin")]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] ChangeOrderStatusDto dto)
    {
        var order = await _dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.DeletedAt == null);

        if (order == null)
            return NotFound();

        var roles = User.Claims
            .Where(c =>
                c.Type == System.Security.Claims.ClaimTypes.Role ||
                c.Type == "role")
            .Select(c => c.Value)
            .ToList();

        if (!roles.Any())
            return Forbid();

        var userIdClaim = User.Claims.FirstOrDefault(c =>
            c.Type == System.Security.Claims.ClaimTypes.NameIdentifier ||
            c.Type == "sub" ||
            c.Type == "userId")?.Value;

        if (!int.TryParse(userIdClaim, out var changedByUserId))
            return Forbid();

        var currentStatus = (OrderStatusEnum)order.OrderStatusId;
        var newStatus = (OrderStatusEnum)dto.Status;

        var canTransition = roles.Any(role =>
            OrderStatusTransitions.CanTransition(currentStatus, newStatus, role));

        if (!canTransition)
            return BadRequest("Invalid status transition.");

        if (newStatus is OrderStatusEnum.Failed or OrderStatusEnum.Cancelled)
        {
            if (string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("A reason is required for failed or cancelled orders.");

            order.FailureReason = dto.Reason.Trim();
        }

        order.OrderStatusId = dto.Status;
        order.UpdatedAt = DateTime.UtcNow;

        if (newStatus == OrderStatusEnum.Submitted && order.SubmittedAt == null)
        {
            order.SubmittedAt = DateTime.UtcNow;
        }

        _dbContext.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = order.OrderId,
            FromStatusId = (int)currentStatus,
            ToStatusId = dto.Status,
            ChangedByUserId = changedByUserId,
            ChangedAt = DateTime.UtcNow,
            Reason = dto.Reason
        });

        await _dbContext.SaveChangesAsync();

        if (newStatus == OrderStatusEnum.Submitted)
        {
            await _processingJobQueueService.QueueSubmissionJobsAsync(order.OrderId);
        }

        if (newStatus == OrderStatusEnum.Approved)
        {
            await _processingJobQueueService.QueueApprovalJobsAsync(order.OrderId);
        }

        return NoContent();
    }

    [HttpGet("{id:int}/history")]
    [Authorize(Roles = "Sales,Operations,Admin")]
    public async Task<IActionResult> GetStatusHistory(int id)
    {
        var orderExists = await _dbContext.Orders
            .AnyAsync(o => o.OrderId == id && o.DeletedAt == null);

        if (!orderExists)
            return NotFound();

        var history = await _dbContext.OrderStatusHistories
            .Include(h => h.FromStatus)
            .Include(h => h.ToStatus)
            .Include(h => h.ChangedByUser)
            .Where(h => h.OrderId == id)
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new OrderStatusHistoryDto
            {
                OrderStatusHistoryId = h.OrderStatusHistoryId,
                FromStatusName = h.FromStatus != null ? h.FromStatus.Name : null,
                ToStatusName = h.ToStatus.Name,
                ChangedByUserName = h.ChangedByUser.FullName,
                ChangedAt = h.ChangedAt,
                Reason = h.Reason
            })
            .ToListAsync();

        return Ok(history);
    }
}