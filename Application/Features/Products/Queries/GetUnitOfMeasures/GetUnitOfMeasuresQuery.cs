using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetUnitOfMeasures;

public class GetUnitOfMeasuresQuery : IRequest<List<UnitOfMeasureDto>>
{
}