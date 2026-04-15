using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationTestController : ControllerBase
{
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Admin access granted." });
    }

    [HttpGet("sales")]
    [Authorize(Policy = "SalesOrAdmin")]
    public IActionResult SalesOrAdmin()
    {
        return Ok(new { message = "Sales or Admin access granted." });
    }

    [HttpGet("operations")]
    [Authorize(Policy = "OperationsOrAdmin")]
    public IActionResult OperationsOrAdmin()
    {
        return Ok(new { message = "Operations or Admin access granted." });
    }
}