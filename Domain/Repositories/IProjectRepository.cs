using Domain.Entities.Organisation;

namespace Domain.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(CancellationToken ct);
    Task<Project?> GetByIdAsync(int id, CancellationToken ct);
}