using Domain.Entities;

namespace Domain.Repositories;

public interface IDocumentRepository
{
    Task<List<Document>> GetByOrderIdAsync(int orderId, CancellationToken ct);
    Task<Document?> GetByIdAsync(int documentId, CancellationToken ct);
}