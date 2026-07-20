using Application.Features.Products.DTOs;
using MediatR;

namespace Application.Features.ProductCategories.Queries.GetProductCategories;

public class GetProductCategoriesQuery : IRequest<List<ProductCategoryDto>>
{
}