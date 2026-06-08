using Application.Features.Customers.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerSummary;

public class GetCustomerSummaryQueryHandler : IRequestHandler<GetCustomerSummaryQuery, CustomerSummaryDto>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerSummaryQueryHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<CustomerSummaryDto> Handle(GetCustomerSummaryQuery request, CancellationToken ct)
    {
        var summary = await _repo.GetSummaryAsync(ct);

        return new CustomerSummaryDto
        {
            TotalCustomers = summary.TotalCustomers,
            ActiveCustomers = summary.ActiveCustomers,
            InactiveCustomers = summary.InactiveCustomers
        };
    }
}
