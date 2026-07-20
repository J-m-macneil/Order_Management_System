using Application.Features.Customers.DTOs;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomerContact;

public class CreateCustomerContactCommand : IRequest<CustomerContactDto>
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public bool IsPrimary { get; set; }
}
