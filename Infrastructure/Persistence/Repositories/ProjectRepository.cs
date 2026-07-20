using Domain.Entities.Organisation;
using Domain.Repositories;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Project>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Projects
            .AsNoTracking()
            .OrderBy(x => x.ProjectCode)
            .ToListAsync(ct);
    }

    public async Task<Project?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ProjectId == id, ct);
    }
}