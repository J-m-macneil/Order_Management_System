using Application.Features.Products.DTOs;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/hazard-classes")]
[Authorize]
public class HazardClassesController : ControllerBase
{
    private readonly IHazardClassRepository _repo;

    public HazardClassesController(IHazardClassRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HazardClassDto>>> Get(CancellationToken ct)
    {
        var result = await _repo.GetAllAsync(ct);

        return Ok(result.Select(x => new HazardClassDto
        {
            HazardClassId = x.HazardClassId,
            Name = x.Name
        }));
    }
}