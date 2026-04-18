using Application.DTOs;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/hazard-classes")]
[Authorize]
public class HazardClassesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public HazardClassesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HazardClassDto>>> Get()
    {
        var hazardClasses = await _dbContext.HazardClasses
            .Select(x => new HazardClassDto
            {
                HazardClassId = x.HazardClassId,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(hazardClasses);
    }
}