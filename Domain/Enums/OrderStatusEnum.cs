namespace Domain.Enums
{
    public enum OrderStatusEnum
    {
        Draft = 1,
        Submitted = 2,
        PendingReview = 3,
        Approved = 4,
        InProcessing = 5,
        AwaitingDispatch = 6,
        Completed = 7,
        Failed = 8,
        Cancelled = 9
    }
}
