using Application.DTOs;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/product-categories")]
[Authorize]
public class ProductCategoriesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductCategoriesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductCategoryDto>>> Get()
    {
        var categories = await _dbContext.ProductCategories
            .Select(x => new ProductCategoryDto
            {
                ProductCategoryId = x.ProductCategoryId,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(categories);
    }
}