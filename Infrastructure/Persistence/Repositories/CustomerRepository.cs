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

    public async Task<int> CountActiveAsync(
        string? searchTerm,
        string? industryType,
        int? paymentTermsDays,
        bool? isActive,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Customers.AsNoTracking(),
                searchTerm,
                industryType,
                paymentTermsDays,
                isActive)
            .CountAsync(ct);
    }

    public async Task<(int TotalCustomers, int ActiveCustomers, int InactiveCustomers)> GetSummaryAsync(
        CancellationToken ct)
    {
        var customers = _db.Customers
            .AsNoTracking()
            .Where(c => c.DeletedAt == null);

        var totalCustomers = await customers.CountAsync(ct);
        var activeCustomers = await customers.CountAsync(c => c.IsActive, ct);
        var inactiveCustomers = await customers.CountAsync(c => !c.IsActive, ct);

        return (totalCustomers, activeCustomers, inactiveCustomers);
    }

    public async Task<List<string>> GetIndustryTypesAsync(CancellationToken ct)
    {
        return await _db.Customers
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.IndustryType != string.Empty)
            .Select(c => c.IndustryType)
            .Distinct()
            .OrderBy(industry => industry)
            .ToListAsync(ct);
    }

    public async Task<List<Customer>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        string? industryType,
        int? paymentTermsDays,
        bool? isActive,
        CancellationToken ct)
    {
        return await ApplyFilters(
                _db.Customers.AsNoTracking(),
                searchTerm,
                industryType,
                paymentTermsDays,
                isActive)
            .OrderBy(c => c.CompanyName)
            .ThenBy(c => c.CustomerId)
            .Skip(skip)
            .Take(take)
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

    private static IQueryable<Customer> ApplyFilters(
        IQueryable<Customer> query,
        string? searchTerm,
        string? industryType,
        int? paymentTermsDays,
        bool? isActive)
    {
        query = query.Where(c => c.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();

            query = query.Where(c =>
                c.AccountNumber.Contains(term) ||
                c.CompanyName.Contains(term) ||
                (c.IndustryType != null && c.IndustryType.Contains(term)) ||
                (c.MainContactName != null && c.MainContactName.Contains(term)) ||
                (c.MainContactEmail != null && c.MainContactEmail.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(industryType))
        {
            query = query.Where(c => c.IndustryType == industryType);
        }

        if (paymentTermsDays.HasValue)
        {
            query = query.Where(c => c.PaymentTermsDays == paymentTermsDays.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        return query;
    }
}
