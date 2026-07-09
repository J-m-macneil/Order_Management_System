using Domain.Enums;

namespace Application.Interfaces;

public interface IOrderStatusWorkflowService
{
    Task MoveToStatusAsync(
        int orderId,
        OrderStatusEnum toStatus,
        string reason,
        CancellationToken cancellationToken);
}
