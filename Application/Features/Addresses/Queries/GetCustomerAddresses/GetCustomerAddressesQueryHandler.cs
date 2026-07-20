using Application.Features.Addresses.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Queries.GetCustomerAddresses;

public class GetCustomerAddressesQueryHandler
    : IRequestHandler<GetCustomerAddressesQuery, List<AddressDto>>
{
    private readonly IAddressRepository _repo;

    public GetCustomerAddressesQueryHandler(IAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AddressDto>> Handle(GetCustomerAddressesQuery request, CancellationToken ct)
    {
        var addresses = await _repo.GetByCustomerAsync(request.CustomerId, ct);

        return addresses
            .OrderBy(x => x.AddressType)
            .ThenBy(x => x.SiteName)
            .Select(x => new AddressDto
            {
                AddressId = x.AddressId,
                CustomerId = x.CustomerId,
                AddressType = x.AddressType,
                SiteName = x.SiteName,
                Line1 = x.Line1,
                Line2 = x.Line2,
                City = x.City,
                County = x.County,
                Postcode = x.Postcode,
                Country = x.Country,
                ContactName = x.ContactName,
                ContactPhone = x.ContactPhone,
                DeliveryInstructions = x.DeliveryInstructions
            })
            .ToList();
    }
}
