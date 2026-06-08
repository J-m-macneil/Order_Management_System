using Application.Common.Models;
using MediatR;

namespace Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : PaginationQuery, IRequest<PagedResult<ProductListDto>>
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsRestricted { get; set; }
    public bool? IsHazardous { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? HazardClassId { get; set; }
}
