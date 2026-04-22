using Application.DTOs;
using Domain.Entities;
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

    public OrdersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
            IsPriorityOrder = dto.IsPriorityOrder,
            IsActive = true
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
            .Include(o => o.OrderItems)
            .Where(o => o.DeletedAt == null)
            .ToListAsync();

        var result = orders.Select(order => new OrderDto
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
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == id && o.DeletedAt == null);

        if (order == null)
            return NotFound();

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
}