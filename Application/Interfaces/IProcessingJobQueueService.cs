namespace Application.Interfaces;

public interface IProcessingJobQueueService
{
    Task QueueSubmissionJobsAsync(int orderId);
    Task QueueApprovalJobsAsync(int orderId);
}
