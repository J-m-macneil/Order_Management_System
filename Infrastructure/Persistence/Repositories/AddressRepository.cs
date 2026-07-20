using Domain.Entities;
using Domain.Entities.Customers;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _db;

    public AddressRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Address>> GetByCustomerAsync(int customerId, CancellationToken ct)
    {
        return await _db.Addresses
            .Where(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Address?> GetByIdAsync(int customerId, int addressId, CancellationToken ct)
    {
        return await _db.Addresses
            .FirstOrDefaultAsync(x =>
                x.AddressId == addressId &&
                x.CustomerId == customerId &&
                x.IsActive &&
                x.DeletedAt == null, ct);
    }

    public async Task AddAsync(Address address, CancellationToken ct)
    {
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Address address, CancellationToken ct)
    {
        address.IsActive = false;
        address.DeletedAt = DateTime.UtcNow;

        _db.Addresses.Update(address);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}
