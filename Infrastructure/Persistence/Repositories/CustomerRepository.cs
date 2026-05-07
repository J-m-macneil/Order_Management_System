using Domain.Entities.Customers;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Customer customer, CancellationToken ct)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Customer?> GetByIdAsync(int customerId, CancellationToken ct)
    {
        return await _db.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c =>
                c.CustomerId == customerId &&
                c.DeletedAt == null, ct);
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Customers
            .Where(c => c.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken ct)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer customer, CancellationToken ct)
    {
        customer.IsActive = false;
        customer.DeletedAt = DateTime.UtcNow;

        _db.Customers.Update(customer);
        await _db.SaveChangesAsync(ct);
    }
}