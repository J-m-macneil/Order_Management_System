using Domain.Repositories;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerIndustryTypes;

public class GetCustomerIndustryTypesQueryHandler : IRequestHandler<GetCustomerIndustryTypesQuery, List<string>>
{
    private readonly ICustomerRepository _repo;

    public GetCustomerIndustryTypesQueryHandler(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<string>> Handle(GetCustomerIndustryTypesQuery request, CancellationToken ct)
    {
        return await _repo.GetIndustryTypesAsync(ct);
    }
}
