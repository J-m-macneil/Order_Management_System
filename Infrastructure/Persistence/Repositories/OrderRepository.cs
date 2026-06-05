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

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
