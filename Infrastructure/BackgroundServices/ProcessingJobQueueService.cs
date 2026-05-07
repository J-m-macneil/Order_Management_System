using Infrastructure.Persistence.Context;

public class ProcessingJobQueueService : IProcessingJobQueueService
{
    private readonly AppDbContext _context;
    private readonly ISystemSettingsService _settings;

    public ProcessingJobQueueService(
        AppDbContext context,
        ISystemSettingsService settings)
    {
        _context = context;
        _settings = settings;
    }

    public async Task QueueSubmissionJobsAsync(int orderId)
    {
        var maxAttempts = await _settings.GetIntAsync("BackgroundJobRetryLimit");

        var jobs = new List<ProcessingJob>
        {
            CreateJob(orderId, "GenerateOrderSummaryDocument", maxAttempts),
            CreateJob(orderId, "CreateSubmissionNotification", maxAttempts)
        };

        _context.ProcessingJobs.AddRange(jobs);
        await _context.SaveChangesAsync();
    }

    public async Task QueueApprovalJobsAsync(int orderId)
    {
        var maxAttempts = await _settings.GetIntAsync("BackgroundJobRetryLimit");

        var jobs = new List<ProcessingJob>
        {
            CreateJob(orderId, "GenerateDeliveryNote", maxAttempts),
            CreateJob(orderId, "PushToLogisticsProvider", maxAttempts),
            CreateJob(orderId, "CreateApprovalNotification", maxAttempts)
        };

        _context.ProcessingJobs.AddRange(jobs);
        await _context.SaveChangesAsync();
    }

    private static ProcessingJob CreateJob(int orderId, string jobType, int maxAttempts)
    {
        return new ProcessingJob
        {
            OrderId = orderId,
            JobType = jobType,
            Status = "Queued",
            AttemptCount = 0,
            MaxAttempts = maxAttempts,
            CreatedAt = DateTime.UtcNow
        };
    }
}