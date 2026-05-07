using Domain.Entities.Orders;

namespace Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);

    Task<Order?> GetByIdAsync(int id, CancellationToken ct);

    Task<List<Order>> GetAllAsync(CancellationToken ct);

    Task<List<OrderStatus>> GetAllStatusesAsync(CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}