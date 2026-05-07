using Domain.Entities.Customers;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

public class CustomerContactRepository : ICustomerContactRepository
{
    private readonly AppDbContext _db;

    public CustomerContactRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CustomerContact>> GetByCustomerAsync(int customerId, CancellationToken ct)
    {
        return await _db.CustomerContacts
            .Where(x => x.CustomerId == customerId && x.IsActive && x.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<CustomerContact?> GetByIdAsync(int customerId, int contactId, CancellationToken ct)
    {
        return await _db.CustomerContacts
            .FirstOrDefaultAsync(x =>
                x.CustomerContactId == contactId &&
                x.CustomerId == customerId &&
                x.DeletedAt == null, ct);
    }

    public async Task AddAsync(CustomerContact contact, CancellationToken ct)
    {
        _db.CustomerContacts.Add(contact);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }
}