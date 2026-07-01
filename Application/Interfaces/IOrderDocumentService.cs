using Domain.Entities.Orders;

namespace Application.Interfaces;

public interface IOrderDocumentService
{
    Task GenerateForJobAsync(
        ProcessingJob job,
        CancellationToken cancellationToken);

    Task<bool> RequiredApprovalDocumentsExistAsync(
        int orderId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> GetMissingApprovalDocumentTypesAsync(
        Order order,
        CancellationToken cancellationToken);

    string GetGenerationJobType(string documentType);
}
