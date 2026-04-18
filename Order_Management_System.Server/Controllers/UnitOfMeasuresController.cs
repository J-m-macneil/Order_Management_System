using Application.DTOs;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/unit-of-measures")]
[Authorize]
public class UnitOfMeasuresController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public UnitOfMeasuresController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitOfMeasureDto>>> Get()
    {
        var units = await _dbContext.UnitsOfMeasure
            .Select(x => new UnitOfMeasureDto
            {
                UnitOfMeasureId = x.UnitOfMeasureId,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(units);
    }
}