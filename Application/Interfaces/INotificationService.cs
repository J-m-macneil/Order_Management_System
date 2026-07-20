namespace Application.Interfaces;

public interface INotificationService
{
    Task CreateOrderSubmittedNotificationAsync(int orderId, CancellationToken cancellationToken);
}
