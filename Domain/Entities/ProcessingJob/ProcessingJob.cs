using Domain.Entities.Orders;

public class ProcessingJob
{
    public int ProcessingJobId { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public string JobType { get; set; } = null!;
    public string Status { get; set; } = "Queued";

    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; } = 3;

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? FailedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastRetryAt { get; set; }
    public DateTime? NextAttemptAt { get; set; }

    public string? PayloadJson { get; set; }
}