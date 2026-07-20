using Application.Features.Carriers.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Carriers.Queries.GetCarriers;

public class GetCarriersQueryHandler : IRequestHandler<GetCarriersQuery, List<CarrierDto>>
{
    private readonly ICarrierRepository _repo;

    public GetCarriersQueryHandler(ICarrierRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<CarrierDto>> Handle(GetCarriersQuery request, CancellationToken ct)
    {
        var carriers = await _repo.GetActiveAsync(ct);

        return carriers
            .OrderBy(x => x.Name)
            .Select(x => new CarrierDto
            {
                CarrierId = x.CarrierId,
                Name = x.Name,
                ContactEmail = x.ContactEmail,
                ServiceType = x.ServiceType
            })
            .ToList();
    }
}