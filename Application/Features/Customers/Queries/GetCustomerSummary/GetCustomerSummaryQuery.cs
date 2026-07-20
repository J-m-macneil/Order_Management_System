using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerSummary;

public class GetCustomerSummaryQuery : IRequest<CustomerSummaryDto>
{
}
