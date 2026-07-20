using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomerContacts;

public class GetCustomerContactsQuery : IRequest<List<CustomerContactDto>>
{
    public int CustomerId { get; set; }
}