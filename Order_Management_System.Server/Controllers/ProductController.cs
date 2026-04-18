using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // =========================
    // GET ALL
    // =========================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductListDto>>> Get()
    {
        var products = await _dbContext.Products
            .Where(x => x.IsActive && x.DeletedAt == null)
            .Select(x => new ProductListDto
            {
                ProductId = x.ProductId,
                SKU = x.SKU,
                ProductName = x.ProductName,
                ProductCategoryName = x.ProductCategory.Name,
                UnitOfMeasureName = x.UnitOfMeasure.Name,
                HazardClassName = x.HazardClass.Name,
                PackSize = x.PackSize,
                BasePrice = x.BasePrice,
                Currency = x.Currency,
                IsRestricted = x.IsRestricted,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return Ok(products);
    }

    // =========================
    // GET BY ID
    // =========================
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _dbContext.Products
            .Where(x => x.ProductId == id && x.DeletedAt == null)
            .Select(x => new ProductDto
            {
                ProductId = x.ProductId,
                SKU = x.SKU,
                ProductName = x.ProductName,
                Description = x.Description,
                ProductCategoryId = x.ProductCategoryId,
                UnitOfMeasureId = x.UnitOfMeasureId,
                PackSize = x.PackSize,
                BasePrice = x.BasePrice,
                Currency = x.Currency,
                HazardClassId = x.HazardClassId,
                UNNumber = x.UNNumber,
                StorageRequirement = x.StorageRequirement,
                RequiresSds = x.RequiresSds,
                IsRestricted = x.IsRestricted,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // =========================
    // CREATE
    // =========================
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = new Product
        {
            SKU = dto.SKU,
            ProductName = dto.ProductName,
            Description = dto.Description,
            ProductCategoryId = dto.ProductCategoryId,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            PackSize = dto.PackSize,
            BasePrice = dto.BasePrice,
            Currency = dto.Currency,
            HazardClassId = dto.HazardClassId,
            UNNumber = dto.UNNumber,
            StorageRequirement = dto.StorageRequirement,
            RequiresSds = dto.RequiresSds,
            IsRestricted = dto.IsRestricted,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return Ok(new ProductDto
        {
            ProductId = product.ProductId,
            SKU = product.SKU,
            ProductName = product.ProductName
        });
    }

    // =========================
    // UPDATE
    // =========================
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

        if (product == null)
            return NotFound();

        product.SKU = dto.SKU;
        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.ProductCategoryId = dto.ProductCategoryId;
        product.UnitOfMeasureId = dto.UnitOfMeasureId;
        product.PackSize = dto.PackSize;
        product.BasePrice = dto.BasePrice;
        product.Currency = dto.Currency;
        product.HazardClassId = dto.HazardClassId;
        product.UNNumber = dto.UNNumber;
        product.StorageRequirement = dto.StorageRequirement;
        product.RequiresSds = dto.RequiresSds;
        product.IsRestricted = dto.IsRestricted;
        product.IsActive = dto.IsActive;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // =========================
    // SOFT DELETE
    // =========================
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(x => x.ProductId == id && x.DeletedAt == null);

        if (product == null)
            return NotFound();

        product.IsActive = false;
        product.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}