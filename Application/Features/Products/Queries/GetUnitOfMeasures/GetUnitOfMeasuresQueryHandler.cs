using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetUnitOfMeasures;

public class GetUnitOfMeasuresQueryHandler
    : IRequestHandler<GetUnitOfMeasuresQuery, List<UnitOfMeasureDto>>
{
    private readonly IUnitOfMeasureRepository _repo;

    public GetUnitOfMeasuresQueryHandler(IUnitOfMeasureRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<UnitOfMeasureDto>> Handle(GetUnitOfMeasuresQuery request, CancellationToken ct)
    {
        var units = await _repo.GetAllAsync(ct);

        return units.Select(x => new UnitOfMeasureDto
        {
            UnitOfMeasureId = x.UnitOfMeasureId,
            Code = x.Code,
            Name = x.Name
        }).ToList();
    }
}