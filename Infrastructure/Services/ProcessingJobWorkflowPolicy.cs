using Application.Common.Exceptions;
using Application.Interfaces;

namespace Infrastructure.Services;

public class ProcessingJobWorkflowPolicy : IProcessingJobWorkflowPolicy
{
    private static readonly string[] OrderedApprovalJobTypes =
    {
        ProcessingJobType.GenerateOrderSummaryDocument,
        ProcessingJobType.GenerateSdsBundle,
        ProcessingJobType.PushToLogisticsProvider
    };

    public int GetProcessingPriority(string jobType)
    {
        return jobType switch
        {
            ProcessingJobType.GenerateOrderSummaryDocument => 10,
            ProcessingJobType.GenerateSdsBundle => 20,
            ProcessingJobType.PushToLogisticsProvider => 30,
            _ => 100
        };
    }

    public bool IsOperatorActionRequiredFailure(ProcessingJob job, Exception exception)
    {
        return exception is OperatorActionRequiredException ||
            (job.JobType == ProcessingJobType.GenerateSdsBundle &&
             exception is IOException or UnauthorizedAccessException);
    }

    public IReadOnlyCollection<string> GetLaterJobTypes(string failedJobType)
    {
        var failedJobPriority = GetProcessingPriority(failedJobType);

        return OrderedApprovalJobTypes
            .Where(jobType => GetProcessingPriority(jobType) > failedJobPriority)
            .ToList();
    }
}
