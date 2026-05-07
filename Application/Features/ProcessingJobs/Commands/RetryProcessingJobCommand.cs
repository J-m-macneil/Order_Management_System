using MediatR;

namespace Application.Features.ProcessingJobs.Commands.RetryProcessingJob;

public class RetryProcessingJobCommand : IRequest
{
    public int Id { get; set; }
}