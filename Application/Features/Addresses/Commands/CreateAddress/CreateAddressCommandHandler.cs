using Application.Features.Addresses.DTOs;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    private readonly IAddressRepository _repo;

    public CreateAddressCommandHandler(IAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken ct)
    {
        var address = new Address
        {
            CustomerId = request.CustomerId,
            AddressType = request.AddressType,
            SiteName = request.SiteName,
            Line1 = request.Line1,
            Line2 = request.Line2,
            City = request.City,
            County = request.County,
            Postcode = request.Postcode,
            Country = request.Country,
            ContactName = request.ContactName,
            ContactPhone = request.ContactPhone,
            DeliveryInstructions = request.DeliveryInstructions,
            IsPrimary = request.IsPrimary,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(address, ct);

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
