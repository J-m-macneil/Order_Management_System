using Domain.Repositories;
using MediatR;

namespace Application.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, Unit>
{
    private readonly IAddressRepository _repo;

    public DeleteAddressCommandHandler(IAddressRepository repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await _repo.GetByIdAsync(request.CustomerId, request.AddressId, ct);

        if (address == null)
            throw new Exception("Address not found");

        await _repo.DeleteAsync(address, ct);

        return Unit.Value;
    }
}
