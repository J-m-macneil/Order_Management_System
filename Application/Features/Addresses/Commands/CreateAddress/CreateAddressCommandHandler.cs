using Application.Features.Addresses.DTOs;
using Application.Interfaces;
using Application.Common.Validation;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    private readonly IAddressRepository _repo;
    private readonly IAuditService _audit;

    public CreateAddressCommandHandler(
        IAddressRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<AddressDto> Handle(CreateAddressCommand request, CancellationToken ct)
    {
        ValidateRequest(request);

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

        await _audit.LogAsync(
            "Address",
            address.AddressId,
            "Created",
            null,
            CreateSnapshot(address),
            $"Address created for customer #{address.CustomerId}: {address.SiteName}.",
            ct);

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

    private static void ValidateRequest(CreateAddressCommand request)
    {
        CommandValidation.PositiveId(request.CustomerId, "Customer");
        CommandValidation.RequiredText(request.AddressType, "Address type", 50);
        CommandValidation.RequiredText(request.SiteName, "Site name", 120);
        CommandValidation.RequiredText(request.Line1, "Address line 1", 120);
        CommandValidation.OptionalText(request.Line2, "Address line 2", 120);
        CommandValidation.RequiredText(request.City, "City", 80);
        CommandValidation.OptionalText(request.County, "County", 80);
        CommandValidation.RequiredText(request.Postcode, "Postcode", 20);
        CommandValidation.RequiredText(request.Country, "Country", 80);
        CommandValidation.OptionalText(request.ContactName, "Contact name", 120);
        CommandValidation.OptionalPhone(request.ContactPhone, "Contact phone");
        CommandValidation.OptionalText(request.DeliveryInstructions, "Delivery instructions", 255);
    }

    private static object CreateSnapshot(Address address)
    {
        return new
        {
            address.AddressId,
            address.CustomerId,
            address.AddressType,
            address.SiteName,
            address.Line1,
            address.Line2,
            address.City,
            address.County,
            address.Postcode,
            address.Country,
            address.ContactName,
            address.ContactPhone,
            address.DeliveryInstructions,
            address.IsPrimary,
            address.IsActive,
            address.DeletedAt
        };
    }
}
