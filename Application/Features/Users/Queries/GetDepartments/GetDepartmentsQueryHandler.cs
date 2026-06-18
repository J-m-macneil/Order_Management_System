using Application.Features.Users.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Users.Queries.GetDepartments;

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IUserRepository _repo;

    public GetDepartmentsQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken ct)
    {
        var departments = await _repo.GetDepartmentsAsync(ct);

        return departments.Select(x => new DepartmentDto
        {
            DepartmentId = x.DepartmentId,
            Name = x.Name
        }).ToList();
    }
}
