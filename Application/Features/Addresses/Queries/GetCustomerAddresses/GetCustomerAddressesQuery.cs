using Application.Features.Addresses.DTOs;
using MediatR;

namespace Application.Features.Addresses.Queries.GetCustomerAddresses;

public class GetCustomerAddressesQuery : IRequest<List<AddressDto>>
{
    public int CustomerId { get; set; }
}