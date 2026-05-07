using Application.Features.Addresses.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandler
    : IRequestHandler<GetAddressByIdQuery, AddressDto?>
{
    private readonly IAddressRepository _repo;

    public GetAddressByIdQueryHandler(IAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<AddressDto?> Handle(GetAddressByIdQuery request, CancellationToken ct)
    {
        var address = await _repo.GetByIdAsync(request.CustomerId, request.AddressId, ct);

        if (address == null)
            return null;

        return new AddressDto
        {
            AddressId = address.AddressId,
            CustomerId = address.CustomerId,
            AddressType = address.AddressType,
            SiteName = address.SiteName,
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            County = address.County,
            Postcode = address.Postcode,
            Country = address.Country,
            ContactName = address.ContactName,
            ContactPhone = address.ContactPhone,
            DeliveryInstructions = address.DeliveryInstructions,
            IsPrimary = address.IsPrimary
        };
    }
}