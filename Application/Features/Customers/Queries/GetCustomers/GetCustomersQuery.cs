using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<List<CustomerDto>>
{
}