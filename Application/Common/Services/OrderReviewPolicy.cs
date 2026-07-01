using Application.Interfaces;
using Domain.Entities.Orders;

namespace Application.Common.Services;

public class OrderReviewPolicy : IOrderReviewPolicy
{
    public bool RequiresManualReview(Order order)
    {
        return order.OrderItems.Any(i =>
            i.DeletedAt == null &&
            i.Product?.IsRestricted == true);
    }

    public IReadOnlyCollection<string> GetReviewReasons(Order order)
    {
        var reasons = new List<string>();

        if (RequiresManualReview(order))
        {
            reasons.Add("Contains restricted product");
        }

        return reasons;
    }
}
