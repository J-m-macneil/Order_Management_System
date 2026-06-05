using Application.Features.ProcessingJobs.Commands.RetryProcessingJob;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/processing-jobs")]
[Authorize]
public class ProcessingJobsController : ControllerBase
{
    private readonly IProcessingJobRepository _repo;
    private readonly IMediator _mediator;

    public ProcessingJobsController(
        IProcessingJobRepository repo,
        IMediator mediator)
    {
        _repo = repo;
        _mediator = mediator;
    }

    [HttpGet("failed")]
    [Authorize(Roles = "Operations,Admin")]
    public async Task<IActionResult> GetFailedJobs(CancellationToken ct)
    {
        var jobs = await _repo.GetFailedJobsAsync(ct);

        var result = jobs.Select(j => new
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
        });

        return Ok(result);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetJobsForOrder(int orderId, CancellationToken ct)
    {
        var jobs = await _repo.GetByOrderIdAsync(orderId, ct);

        var result = jobs.Select(j => new
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
        });

        return Ok(result);
    }

    [HttpPost("{id:int}/retry")]
    [Authorize(Roles = "Operations,Admin")]
    public async Task<IActionResult> RetryJob(int id, CancellationToken ct)
    {
        await _mediator.Send(new RetryProcessingJobCommand
        {
            Id = id
        }, ct);

        return NoContent();
    }
}
