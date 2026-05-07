using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}