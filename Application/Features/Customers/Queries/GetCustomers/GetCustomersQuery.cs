using Application.Common.Models;
using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQuery : PaginationQuery, IRequest<PagedResult<CustomerDto>>
{
}
