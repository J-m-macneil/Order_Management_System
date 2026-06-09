using Domain.Entities.Customers;

namespace Domain.Repositories;

public interface ICustomerContactRepository
{
    Task<List<CustomerContact>> GetByCustomerAsync(int customerId, CancellationToken ct);
    Task<CustomerContact?> GetByIdAsync(int customerId, int contactId, CancellationToken ct);
    Task AddAsync(CustomerContact contact, CancellationToken ct);
    Task ClearPrimaryForCustomerAsync(int customerId, int? excludingContactId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
