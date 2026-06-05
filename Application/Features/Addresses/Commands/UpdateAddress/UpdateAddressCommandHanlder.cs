using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHanlder : IRequestHandler<UpdateAddressCommand, Unit>
{
    private readonly IAddressRepository _repo;

    public UpdateAddressCommandHanlder(IAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        var address = await _repo.GetByIdAsync(request.CustomerId, request.AddressId, ct);

        if (address == null)
            throw new Exception("Address not found");

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

        return Unit.Value;
    }
}
