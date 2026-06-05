using MediatR;

namespace Application.Features.Addresses.Commands.DeleteAddress;

public class DeleteAddressCommand : IRequest<Unit>
{
    public int CustomerId { get; set; }
    public int AddressId { get; set; }
}
