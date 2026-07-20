using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerIndustryTypes;

public class GetCustomerIndustryTypesQuery : IRequest<List<string>>
{
}
