using Domain.Enums;

namespace Domain.Entities.Orders
{
    public class StatusTransitionRule
    {
        public OrderStatusEnum From { get; set; }
        public OrderStatusEnum To { get; set; }
        public List<string> AllowedRoles { get; set; } = new();
    }
}
