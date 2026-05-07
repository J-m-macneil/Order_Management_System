using Domain.Entities.Customers;

namespace Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);

    Task<Customer?> GetByIdAsync(int customerId, CancellationToken ct);

    Task<List<Customer>> GetAllAsync(CancellationToken ct);

    Task UpdateAsync(Customer customer, CancellationToken ct);

    Task DeleteAsync(Customer customer, CancellationToken ct);
}