using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/carriers")]
[Authorize]
public class CarriersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CarriersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Carrier>>> GetAll()
    {
        var carriers = await _dbContext.Carriers
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(carriers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Carrier>> GetById(int id)
    {
        var carrier = await _dbContext.Carriers
            .FirstOrDefaultAsync(x => x.CarrierId == id && x.IsActive);

        if (carrier == null)
            return NotFound();

        return Ok(carrier);
    }
}