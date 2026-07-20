namespace Application.Interfaces;

public interface IProcessingJobWorkflowPolicy
{
    int GetProcessingPriority(string jobType);

    bool IsOperatorActionRequiredFailure(ProcessingJob job, Exception exception);
}
