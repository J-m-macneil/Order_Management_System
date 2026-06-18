using Domain.Entities;
using Domain.Entities.Orders;
using Domain.Entities.Status;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await _db.Orders.AddAsync(order, ct);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderStatus)
            .Include(o => o.Warehouse)
            .Include(o => o.Carrier)
            .Include(o => o.Project)
            .Include(o => o.CreatedByUser)
            .Include(o => o.AssignedToUser)
            .Include(o => o.ProcessingJobs)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)

            .Include(o => o.OrderStatusHistory)
                .ThenInclude(h => h.ToStatus)

            .Include(o => o.OrderStatusHistory)
                .ThenInclude(h => h.FromStatus)

            .Include(o => o.OrderStatusHistory)
                .ThenInclude(h => h.ChangedByUser)

            .FirstOrDefaultAsync(o => o.OrderId == id, ct);
    }

    public void RemoveItems(IEnumerable<OrderItem> items)
    {
        _db.OrderItems.RemoveRange(items);
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderStatus)
            .Include(o => o.Warehouse)
            .Include(o => o.Carrier)
            .Include(o => o.Project)
            .Include(o => o.OrderItems)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<OrderStatus>> GetAllStatusesAsync(CancellationToken ct)
    {
        return await _db.OrderStatuses
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<int> CountActiveAsync(
        string? searchTerm,
        int? orderStatusId,
        bool? isPriorityOrder,
        DateTime? requestedDeliveryFrom,
        DateTime? requestedDeliveryTo,
        DateTime? createdFrom,
        DateTime? createdTo,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Orders.AsNoTracking(),
                searchTerm,
                orderStatusId,
                isPriorityOrder,
                requestedDeliveryFrom,
                requestedDeliveryTo,
                createdFrom,
                createdTo)
            .CountAsync(ct);
    }

    public async Task<List<Order>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        int? orderStatusId,
        bool? isPriorityOrder,
        DateTime? requestedDeliveryFrom,
        DateTime? requestedDeliveryTo,
        DateTime? createdFrom,
        DateTime? createdTo,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Orders
                    .AsNoTracking()
                    .Include(o => o.Customer)
                    .Include(o => o.OrderStatus)
                    .Include(o => o.Warehouse)
                    .Include(o => o.Carrier)
                    .Include(o => o.Project)
                    .Include(o => o.OrderItems)
                    .Include(o => o.ProcessingJobs),
                searchTerm,
                orderStatusId,
                isPriorityOrder,
                requestedDeliveryFrom,
                requestedDeliveryTo,
                createdFrom,
                createdTo)
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.OrderId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }

    private static IQueryable<Order> ApplyFilters(
        IQueryable<Order> query,
        string? searchTerm,
        int? orderStatusId,
        bool? isPriorityOrder,
        DateTime? requestedDeliveryFrom,
        DateTime? requestedDeliveryTo,
        DateTime? createdFrom,
        DateTime? createdTo)
    {
        query = query.Where(x => x.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(o =>
                o.OrderNumber.Contains(term) ||
                (o.Customer != null && o.Customer.CompanyName.Contains(term)) ||
                o.CustomerId.ToString().Contains(term) ||
                (o.PurchaseOrderReference != null && o.PurchaseOrderReference.Contains(term)));
        }

        if (orderStatusId == 8)
        {
            query = query.Where(o =>
                o.OrderStatusId == 8 ||
                o.ProcessingJobs.Any(j => j.Status == "Failed"));
        }
        else if (orderStatusId.HasValue)
        {
            query = query.Where(o => o.OrderStatusId == orderStatusId.Value);
        }

        if (isPriorityOrder.HasValue)
        {
            query = query.Where(o => o.IsPriorityOrder == isPriorityOrder.Value);
        }

        if (requestedDeliveryFrom.HasValue)
        {
            query = query.Where(o => o.RequestedDeliveryDate >= requestedDeliveryFrom.Value);
        }

        if (requestedDeliveryTo.HasValue)
        {
            query = query.Where(o => o.RequestedDeliveryDate <= requestedDeliveryTo.Value);
        }

        if (createdFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= createdFrom.Value);
        }

        if (createdTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= createdTo.Value);
        }

        return query;
    }
}
