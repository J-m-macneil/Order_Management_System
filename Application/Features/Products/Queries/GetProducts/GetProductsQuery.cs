using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<List<ProductListDto>> { }