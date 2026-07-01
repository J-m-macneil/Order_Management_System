namespace Application.Interfaces;

public interface IProcessingJobProcessor
{
    Task ProcessNextBatchAsync(CancellationToken cancellationToken);
}
