using Application.Features.Projects.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler
    : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    private readonly IProjectRepository _repo;

    public GetProjectsQueryHandler(IProjectRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var projects = await _repo.GetAllAsync(ct);

        return projects
            .OrderBy(x => x.ProjectCode)
            .Select(x => new ProjectDto
            {
                ProjectId = x.ProjectId,
                ProjectCode = x.ProjectCode,
                Name = x.ProjectName
            })
            .ToList();
    }
}