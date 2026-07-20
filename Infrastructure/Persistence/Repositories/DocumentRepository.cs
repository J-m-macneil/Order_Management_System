using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _db;

    public DocumentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Document>> GetByOrderIdAsync(int orderId, CancellationToken ct)
    {
        return await _db.Documents
            .Where(d => d.OrderId == orderId)
            .OrderByDescending(d => d.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Document?> GetByIdAsync(int documentId, CancellationToken ct)
    {
        return await _db.Documents
            .FirstOrDefaultAsync(d => d.DocumentId == documentId, ct);
    }
}