using Application.Features.Carriers.DTOs;
using Application.Features.Carriers.Queries.GetCarrierById;
using Domain.Repositories;
using MediatR;

public class GetCarrierByIdQueryHandler : IRequestHandler<GetCarrierByIdQuery, CarrierDto?>
{
    private readonly ICarrierRepository _repo;

    public GetCarrierByIdQueryHandler(ICarrierRepository repo)
    {
        _repo = repo;
    }

    public async Task<CarrierDto?> Handle(GetCarrierByIdQuery request, CancellationToken ct)
    {
        var carrier = await _repo.GetByIdAsync(request.CarrierId, ct);

        if (carrier == null)
            return null;

        return new CarrierDto
        {
            CarrierId = carrier.CarrierId,
            Name = carrier.Name,
            ContactEmail = carrier.ContactEmail,
            ServiceType = carrier.ServiceType
        };
    }
}