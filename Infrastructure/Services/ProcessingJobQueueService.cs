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

    public async Task QueueApprovalJobsAsync(int orderId)
    {
        var maxAttempts = await _settings.GetIntAsync("BackgroundJobRetryLimit");
        var requiredJobTypes = await GetRequiredApprovalJobTypesAsync(orderId);

        foreach (var jobType in requiredJobTypes)
        {
            if (await ShouldQueueJobAsync(orderId, jobType))
            {
                _context.ProcessingJobs.Add(CreateJob(orderId, jobType, maxAttempts));
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<string>> GetRequiredApprovalJobTypesAsync(int orderId)
    {
        var jobTypes = new List<string>
        {
            ProcessingJobType.GenerateOrderSummaryDocument
        };

        var requiresSdsBundle = await _context.OrderItems
            .Where(i => i.OrderId == orderId && i.DeletedAt == null)
            .AnyAsync(i => i.Product.RequiresSds);

        if (requiresSdsBundle)
        {
            jobTypes.Add(ProcessingJobType.GenerateSdsBundle);
        }

        jobTypes.Add(ProcessingJobType.PushToLogisticsProvider);

        return jobTypes;
    }

    private async Task<bool> ShouldQueueJobAsync(int orderId, string jobType)
    {
        if (await DocumentExistsForJobAsync(orderId, jobType))
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

    private async Task<bool> DocumentExistsForJobAsync(int orderId, string jobType)
    {
        var documentType = jobType switch
        {
            ProcessingJobType.GenerateOrderSummaryDocument => DocumentType.OrderSummary,
            ProcessingJobType.GenerateSdsBundle => DocumentType.SafetyDataSheetBundle,
            _ => null
        };

        return documentType != null &&
            await _context.Documents.AnyAsync(d =>
                d.OrderId == orderId &&
                d.DocumentType == documentType);
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
