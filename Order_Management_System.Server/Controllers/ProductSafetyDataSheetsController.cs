using Application.DTOs;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/products/{productId}/sds")]
[Authorize]
public class ProductSafetyDataSheetsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProductSafetyDataSheetsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SafetyDataSheetDto>>> Get(int productId)
    {
        var items = await _dbContext.SafetyDataSheets
            .Where(x => x.ProductId == productId && x.IsActive && x.DeletedAt == null)
            .Select(x => new SafetyDataSheetDto
            {
                SafetyDataSheetId = x.SafetyDataSheetId,
                ProductId = x.ProductId,
                FileName = x.FileName,
                FilePath = x.FilePath,
                Version = x.Version,
                EffectiveDate = x.EffectiveDate,
                UploadedAt = x.UploadedAt,
                UploadedByUserId = x.UploadedByUserId
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<SafetyDataSheetDto>> Create(int productId, [FromBody] CreateSafetyDataSheetDto dto)
    {
        var productExists = await _dbContext.Products
            .AnyAsync(x => x.ProductId == productId && x.DeletedAt == null);

        if (!productExists)
            return NotFound("Product not found.");

        var userExists = await _dbContext.Users
            .AnyAsync(x => x.UserId == dto.UploadedByUserId);

        if (!userExists)
            return BadRequest("UploadedByUserId is invalid.");

        var item = new SafetyDataSheet
        {
            ProductId = productId,
            FileName = dto.FileName,
            FilePath = dto.FilePath,
            Version = dto.Version,
            EffectiveDate = dto.EffectiveDate,
            UploadedAt = dto.UploadedAt,
            UploadedByUserId = dto.UploadedByUserId,
            IsActive = true,
            DeletedAt = null
        };

        _dbContext.SafetyDataSheets.Add(item);
        await _dbContext.SaveChangesAsync();

        return Ok(new SafetyDataSheetDto
        {
            SafetyDataSheetId = item.SafetyDataSheetId,
            ProductId = item.ProductId,
            FileName = item.FileName,
            FilePath = item.FilePath,
            Version = item.Version,
            EffectiveDate = item.EffectiveDate,
            UploadedAt = item.UploadedAt,
            UploadedByUserId = item.UploadedByUserId
        });
    }

    [HttpPut("{sdsId}")]
    public async Task<IActionResult> Update(int productId, int sdsId, [FromBody] UpdateSafetyDataSheetDto dto)
    {
        var item = await _dbContext.SafetyDataSheets
            .FirstOrDefaultAsync(x => x.SafetyDataSheetId == sdsId && x.ProductId == productId && x.DeletedAt == null);

        if (item == null)
            return NotFound();

        var userExists = await _dbContext.Users
            .AnyAsync(x => x.UserId == dto.UploadedByUserId);

        if (!userExists)
            return BadRequest("UploadedByUserId is invalid.");

        item.FileName = dto.FileName;
        item.FilePath = dto.FilePath;
        item.Version = dto.Version;
        item.EffectiveDate = dto.EffectiveDate;
        item.UploadedAt = dto.UploadedAt;
        item.UploadedByUserId = dto.UploadedByUserId;
        item.IsActive = dto.IsActive;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{sdsId}")]
    public async Task<IActionResult> Delete(int productId, int sdsId)
    {
        var item = await _dbContext.SafetyDataSheets
            .FirstOrDefaultAsync(x => x.SafetyDataSheetId == sdsId && x.ProductId == productId && x.DeletedAt == null);

        if (item == null)
            return NotFound();

        item.IsActive = false;
        item.DeletedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}