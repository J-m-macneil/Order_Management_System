using Application.Features.ProductCategories.Queries.GetProductCategories;
using Application.Features.Products.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductCategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> Get()
    {
        var result = await _mediator.Send(new GetProductCategoriesQuery());
        return Ok(result);
    }
}