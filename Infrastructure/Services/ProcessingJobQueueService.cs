using Application.Interfaces;
using Domain.Entities.Documents;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

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
            CreateJob(orderId, ProcessingJobType.GenerateOrderSummaryDocument, maxAttempts),
            CreateJob(orderId, ProcessingJobType.CreateSubmissionNotification, maxAttempts)
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
                i.Product.RequiresSds))
            .FirstAsync();

        var jobs = new List<ProcessingJob>();

        if (await ShouldQueueDocumentJobAsync(orderId, DocumentType.OrderSummary, ProcessingJobType.GenerateOrderSummaryDocument))
        {
            jobs.Add(CreateJob(orderId, ProcessingJobType.GenerateOrderSummaryDocument, maxAttempts));
        }

        if (requiresSdsBundle &&
            await ShouldQueueDocumentJobAsync(orderId, DocumentType.SafetyDataSheetBundle, ProcessingJobType.GenerateSdsBundle))
        {
            jobs.Add(CreateJob(orderId, ProcessingJobType.GenerateSdsBundle, maxAttempts));
        }

        jobs.Add(CreateJob(orderId, ProcessingJobType.PushToLogisticsProvider, maxAttempts));
        jobs.Add(CreateJob(orderId, ProcessingJobType.CreateApprovalNotification, maxAttempts));

        _context.ProcessingJobs.AddRange(jobs);
        await _context.SaveChangesAsync();
    }

    private async Task<bool> ShouldQueueDocumentJobAsync(int orderId, string documentType, string jobType)
    {
        var documentExists = await _context.Documents.AnyAsync(d =>
            d.OrderId == orderId &&
            d.DocumentType == documentType);

        if (documentExists)
        {
            return false;
        }

        var jobExists = await _context.ProcessingJobs.AnyAsync(j =>
            j.OrderId == orderId &&
            j.JobType == jobType &&
            (j.Status == ProcessingJobStatus.Queued ||
             j.Status == ProcessingJobStatus.Processing ||
             j.Status == ProcessingJobStatus.Completed));

        return !jobExists;
    }

    private static ProcessingJob CreateJob(int orderId, string jobType, int maxAttempts)
    {
        return new ProcessingJob
        {
            OrderId = orderId,
            JobType = jobType,
            Status = ProcessingJobStatus.Queued,
            AttemptCount = 0,
            MaxAttempts = maxAttempts,
            CreatedAt = DateTime.UtcNow
        };
    }
}
