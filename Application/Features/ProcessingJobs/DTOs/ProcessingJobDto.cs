namespace Application.Features.ProcessingJobs.DTOs;

public class ProcessingJobDto
{
    public int ProcessingJobId { get; set; }
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string JobType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? LastRetryAt { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public string? PayloadJson { get; set; }

    public static ProcessingJobDto FromEntity(ProcessingJob job, string? orderNumber = null)
    {
        return new ProcessingJobDto
        {
            ProcessingJobId = job.ProcessingJobId,
            OrderId = job.OrderId,
            OrderNumber = orderNumber,
            JobType = job.JobType,
            Status = job.Status,
            AttemptCount = job.AttemptCount,
            MaxAttempts = job.MaxAttempts,
            ErrorMessage = job.ErrorMessage,
            CreatedAt = AsUtc(job.CreatedAt),
            StartedAt = AsUtc(job.StartedAt),
            CompletedAt = AsUtc(job.CompletedAt),
            FailedAt = AsUtc(job.FailedAt),
            LastRetryAt = AsUtc(job.LastRetryAt),
            NextAttemptAt = AsUtc(job.NextAttemptAt),
            PayloadJson = job.PayloadJson
        };
    }

    private static DateTime AsUtc(DateTime value)
    {
        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private static DateTime? AsUtc(DateTime? value)
    {
        return value.HasValue
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
            : null;
    }
}
