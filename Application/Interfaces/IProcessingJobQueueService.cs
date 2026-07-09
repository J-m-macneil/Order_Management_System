namespace Application.Interfaces;

public interface IProcessingJobQueueService
{
    Task QueueApprovalJobsAsync(int orderId);
}
