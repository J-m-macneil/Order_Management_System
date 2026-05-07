using Application.Features.Products.Queries.GetProductById;
using Application.Features.Projects.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
{
    private readonly IProjectRepository _repo;

    public GetProjectByIdQueryHandler(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken ct)
    {
        var project = await _repo.GetByIdAsync(request.ProjectId, ct);

        if (project == null)
            return null;

        return new ProjectDto
        {
            ProjectId = project.ProjectId,
            ProjectCode = project.ProjectCode,
            Name = project.ProjectName
        };
    }
}