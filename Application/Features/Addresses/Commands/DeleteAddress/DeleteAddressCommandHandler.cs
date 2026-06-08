using Application.Interfaces;
using Domain.Entities.Customers;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Unit>
{
    private readonly IAddressRepository _repo;
    private readonly IAuditService _audit;

    public DeleteAddressCommandHandler(
        IAddressRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await _repo.GetByIdAsync(request.CustomerId, request.AddressId, ct);

        if (address == null)
            throw new Exception("Address not found");

        var oldValues = CreateSnapshot(address);

        await _repo.DeleteAsync(address, ct);

        await _audit.LogAsync(
            "Address",
            address.AddressId,
            "Deleted",
            oldValues,
            CreateSnapshot(address),
            $"Address deleted for customer #{address.CustomerId}: {address.SiteName}.",
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
