using Domain.Entities.Orders;
using Domain.Enums;

namespace Application.Features.Orders;

internal static class OrderEffectiveStatus
{
    public static OrderEffectiveStatusResult From(Order order)
    {
        var failedProcessingJobCount = order.ProcessingJobs.Count(j =>
            j.Status == ProcessingJobStatus.Failed);

        if (failedProcessingJobCount > 0)
        {
            return new OrderEffectiveStatusResult(
                (int)OrderStatusEnum.Failed,
                OrderStatusEnum.Failed.ToString(),
                failedProcessingJobCount);
        }

        return new OrderEffectiveStatusResult(
            order.OrderStatusId,
            order.OrderStatus?.Name,
            failedProcessingJobCount);
    }
}

internal sealed record OrderEffectiveStatusResult(
    int StatusId,
    string? StatusName,
    int FailedProcessingJobCount);
