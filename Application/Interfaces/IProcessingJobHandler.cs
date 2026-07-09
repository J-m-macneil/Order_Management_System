public interface IProcessingJobHandler
{
    string JobType { get; }

    Task HandleAsync(ProcessingJob job, CancellationToken cancellationToken);
}
