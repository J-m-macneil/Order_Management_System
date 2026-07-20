using Application.Features.Users.Queries.GetUsers;
using Domain.Entities.Identity;
using Domain.Entities.Organisation;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithQuery_ReturnsPagedUsers()
    {
        // Arrange
        var repo = Substitute.For<IUserRepository>();
        var handler = new GetUsersQueryHandler(repo);
        var query = new GetUsersQuery
        {
            PageNumber = 2,
            PageSize = 10,
            SearchTerm = "alex",
            RoleId = 1,
            IsActive = true
        };

        var users = new List<User>
        {
            new()
            {
                UserId = 5,
                FirstName = "Alex",
                LastName = "Morgan",
                FullName = "Alex Morgan",
                Username = "alex.morgan",
                Email = "alex.morgan@example.com",
                RoleId = 1,
                Role = new Role { RoleId = 1, Name = "Admin" },
                DepartmentId = 2,
                Department = new Department { DepartmentId = 2, Name = "IT" },
                JobTitle = "Administrator",
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 2)
            }
        };

        repo.CountAsync(query.SearchTerm, query.RoleId, query.IsActive, Arg.Any<CancellationToken>())
            .Returns(11);
        repo.GetPagedAsync(query.SearchTerm, query.RoleId, query.IsActive, query.Skip, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(users);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(11);
        result.TotalPages.Should().Be(2);
        result.Items.Should().ContainSingle();

        var user = result.Items.Single();
        user.UserId.Should().Be(5);
        user.FullName.Should().Be("Alex Morgan");
        user.Role.Should().Be("Admin");
        user.Department.Should().Be("IT");
    }
}
