using Application.Interfaces;
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
        address.IsPrimary = request.IsPrimary;

        await _repo.SaveChangesAsync(ct);

        await _audit.LogAsync(
            "Address",
            address.AddressId,
            "Updated",
            oldValues,
            CreateSnapshot(address),
            $"Address updated for customer #{address.CustomerId}: {address.SiteName}.",
            ct);

        return Unit.Value;
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
