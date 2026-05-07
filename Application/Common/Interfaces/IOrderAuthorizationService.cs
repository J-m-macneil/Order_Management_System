using Domain.Enums;

public interface IOrderAuthorizationService
{
    bool CanTransition(OrderStatusEnum from, OrderStatusEnum to, List<string> userRoles);
}