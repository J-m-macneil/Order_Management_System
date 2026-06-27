using Domain.Entities.Customers;

namespace Domain.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken ct);

    Task<Customer?> GetByIdAsync(int customerId, CancellationToken ct);

    Task<List<Customer>> GetAllAsync(CancellationToken ct);

    Task<int> CountActiveAsync(
        string? searchTerm,
        string? industryType,
        int? paymentTermsDays,
        bool? isActive,
        CancellationToken ct);

    Task<(int TotalCustomers, int ActiveCustomers, int InactiveCustomers)> GetSummaryAsync(
        CancellationToken ct);

    Task<List<string>> GetIndustryTypesAsync(CancellationToken ct);

    Task<List<Customer>> GetPagedAsync(
        int skip,
        int take,
        string? searchTerm,
        string? industryType,
        int? paymentTermsDays,
        bool? isActive,
        CancellationToken ct);

    Task UpdateAsync(Customer customer, CancellationToken ct);

    Task DeleteAsync(Customer customer, CancellationToken ct);
}
