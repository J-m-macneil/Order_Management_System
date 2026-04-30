using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/processing-jobs")]
[Authorize(Roles = "Operations,Admin")]
public class ProcessingJobsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ProcessingJobsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedJobs()
    {
        var jobs = await _dbContext.ProcessingJobs
            .Include(j => j.Order)
            .Where(j => j.Status == "Failed")
            .OrderByDescending(j => j.FailedAt ?? j.CreatedAt)
            .Select(j => new
            {
                j.ProcessingJobId,
                j.OrderId,
                OrderNumber = j.Order.OrderNumber,
                j.JobType,
                j.Status,
                j.AttemptCount,
                j.MaxAttempts,
                j.ErrorMessage,
                j.CreatedAt,
                j.StartedAt,
                j.CompletedAt,
                j.FailedAt,
                j.LastRetryAt,
                j.NextAttemptAt,
                j.PayloadJson
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetJobsForOrder(int orderId)
    {
        var orderExists = await _dbContext.Orders
            .AnyAsync(o => o.OrderId == orderId && o.DeletedAt == null);

        if (!orderExists)
            return NotFound();

        var jobs = await _dbContext.ProcessingJobs
            .Where(j => j.OrderId == orderId)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new
            {
                j.ProcessingJobId,
                j.OrderId,
                j.JobType,
                j.Status,
                j.AttemptCount,
                j.MaxAttempts,
                j.ErrorMessage,
                j.CreatedAt,
                j.StartedAt,
                j.CompletedAt,
                j.FailedAt,
                j.LastRetryAt,
                j.NextAttemptAt,
                j.PayloadJson
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpPost("{id:int}/retry")]
    public async Task<IActionResult> RetryJob(int id)
    {
        var job = await _dbContext.ProcessingJobs
            .FirstOrDefaultAsync(j => j.ProcessingJobId == id);

        if (job == null)
            return NotFound();

        if (job.Status != "Failed")
            return BadRequest("Only failed jobs can be retried.");

        job.Status = "Queued";
        job.NextAttemptAt = DateTime.UtcNow;
        job.ErrorMessage = null;
        job.FailedAt = null;

        // optional: reset attempts
        job.AttemptCount = 0;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}