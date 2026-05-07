using Application.Features.Addresses.DTOs;
using MediatR;

namespace Application.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdQuery : IRequest<AddressDto?>
{
    public int CustomerId { get; set; }
    public int AddressId { get; set; }
}