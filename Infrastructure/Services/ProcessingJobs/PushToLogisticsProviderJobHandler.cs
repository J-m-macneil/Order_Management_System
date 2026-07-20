using Application.Interfaces;
using Domain.Enums;

namespace Infrastructure.Services.ProcessingJobs;

public class PushToLogisticsProviderJobHandler : IProcessingJobHandler
{
    private readonly IOrderStatusWorkflowService _orderStatusWorkflow;

    public PushToLogisticsProviderJobHandler(IOrderStatusWorkflowService orderStatusWorkflow)
    {
        _orderStatusWorkflow = orderStatusWorkflow;
    }

    public string JobType => ProcessingJobType.PushToLogisticsProvider;

    public async Task HandleAsync(ProcessingJob job, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        await _orderStatusWorkflow.MoveToStatusAsync(
            job.OrderId,
            OrderStatusEnum.AwaitingDispatch,
            "Order moved to Awaiting Dispatch after simulated logistics provider push.",
            cancellationToken);
    }
}
