using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQuery : IRequest<CustomerDto?>
{
    public int CustomerId { get; set; }
}