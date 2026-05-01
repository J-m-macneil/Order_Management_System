using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderDocumentGenerator
{
    Task<Document> GenerateAsync(
        int orderId,
        string documentType,
        CancellationToken cancellationToken = default);
}