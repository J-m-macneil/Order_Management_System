using Domain.Entities;
using Domain.Entities.Orders;

namespace Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);

    Task<Order?> GetByIdAsync(int id, CancellationToken ct);

    Task<List<Order>> GetAllAsync(CancellationToken ct);

    Task<List<OrderStatus>> GetAllStatusesAsync(CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);

    Task<int> CountActiveAsync(
        string? searchTerm,
        int? orderStatusId,
        bool? isPriorityOrder,
        DateTime? requestedDeliveryFrom,
        DateTime? requestedDeliveryTo,
        DateTime? createdFrom,
        DateTime? createdTo,
        CancellationToken ct);

    Task<List<Order>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        int? orderStatusId,
        bool? isPriorityOrder,
        DateTime? requestedDeliveryFrom,
        DateTime? requestedDeliveryTo,
        DateTime? createdFrom,
        DateTime? createdTo,
        CancellationToken ct);
}
