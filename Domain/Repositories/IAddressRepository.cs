using Domain.Entities;
using Domain.Entities.Customers;

namespace Domain.Repositories;

public interface IAddressRepository
{
    Task<List<Address>> GetByCustomerAsync(int customerId, CancellationToken ct);
    Task<Address?> GetByIdAsync(int customerId, int addressId, CancellationToken ct);
    Task AddAsync(Address address, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}