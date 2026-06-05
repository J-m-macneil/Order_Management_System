using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

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
        var requiresSdsBundle = await _context.Orders
            .Where(o => o.OrderId == orderId)
            .Select(o => o.OrderItems.Any(i =>
                i.DeletedAt == null &&
                (i.Product.RequiresSds || i.Product.IsRestricted)))
            .FirstAsync();

        var jobs = new List<ProcessingJob>
        {
            CreateJob(orderId, "GenerateDeliveryNote", maxAttempts)
        };

        if (requiresSdsBundle)
        {
            jobs.Add(CreateJob(orderId, "GenerateSdsBundle", maxAttempts));
        }

        jobs.Add(CreateJob(orderId, "PushToLogisticsProvider", maxAttempts));
        jobs.Add(CreateJob(orderId, "CreateApprovalNotification", maxAttempts));

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
