using Application.Common.Models;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : PaginationQuery, IRequest<PagedResult<ProductListDto>> { }