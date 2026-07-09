using Domain.Entities.Orders;

namespace Application.Interfaces;

public interface IOrderDocumentService
{
    Task GenerateAsync(
        int orderId,
        string documentType,
        CancellationToken cancellationToken);

    Task<bool> RequiredApprovalDocumentsExistAsync(
        int orderId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> GetMissingApprovalDocumentTypesAsync(
        Order order,
        CancellationToken cancellationToken);
}
