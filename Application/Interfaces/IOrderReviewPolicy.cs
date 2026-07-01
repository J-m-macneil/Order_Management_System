using Domain.Entities.Orders;

namespace Application.Interfaces;

public interface IOrderReviewPolicy
{
    bool RequiresManualReview(Order order);

    IReadOnlyCollection<string> GetReviewReasons(Order order);
}
