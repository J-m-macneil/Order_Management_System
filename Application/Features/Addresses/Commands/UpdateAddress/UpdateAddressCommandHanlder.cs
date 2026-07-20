using Application.Interfaces;
using Application.Common.Validation;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHanlder : IRequestHandler<UpdateAddressCommand, Unit>
{
    private readonly IAddressRepository _repo;
    private readonly IAuditService _audit;

    public UpdateAddressCommandHanlder(
        IAddressRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        ValidateRequest(request);

        var address = await _repo.GetByIdAsync(request.CustomerId, request.AddressId, ct);

        if (address == null)
            throw new Exception("Address not found");

        var oldValues = CreateSnapshot(address);

        address.AddressType = request.AddressType;
        address.SiteName = request.SiteName;
        address.Line1 = request.Line1;
        address.Line2 = request.Line2;
        address.City = request.City;
        address.County = request.County;
        address.Postcode = request.Postcode;
        address.Country = request.Country;
        address.ContactName = request.ContactName;
        address.ContactPhone = request.ContactPhone;
        address.DeliveryInstructions = request.DeliveryInstructions;

        await _repo.SaveChangesAsync(ct);

        var newValues = CreateSnapshot(address);

        await _audit.LogAsync(
            "Address",
            address.AddressId,
            "Updated",
            oldValues,
            newValues,
            $"Address updated: {address.SiteName} for customer #{address.CustomerId}.",
            ct);

        return Unit.Value;
    }

    private static void ValidateRequest(UpdateAddressCommand request)
    {
        CommandValidation.PositiveId(request.CustomerId, "Customer");
        CommandValidation.PositiveId(request.AddressId, "Address");
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
            address.IsActive,
            address.DeletedAt
        };
    }
}
