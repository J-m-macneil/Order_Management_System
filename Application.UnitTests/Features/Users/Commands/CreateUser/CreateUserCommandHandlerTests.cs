using Application.Features.Users.Commands.CreateUser;
using Application.Interfaces;
using Domain.Entities.Identity;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsUserReturnsDtoAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IUserRepository>();
        var passwordService = Substitute.For<IPasswordService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new CreateUserCommandHandler(repo, passwordService, audit);
        var command = CreateValidCommand();
        User? savedUser = null;

        repo.UsernameExistsAsync(command.Username, null, Arg.Any<CancellationToken>())
            .Returns(false);
        repo.EmailExistsAsync(command.Email, null, Arg.Any<CancellationToken>())
            .Returns(false);
        repo.RoleExistsAsync(command.RoleId, Arg.Any<CancellationToken>())
            .Returns(true);
        repo.DepartmentExistsAsync(command.DepartmentId, Arg.Any<CancellationToken>())
            .Returns(true);
        passwordService.HashPassword(command.Password)
            .Returns("hashed-password");

        repo.AddAsync(Arg.Do<User>(user =>
        {
            user.UserId = 42;
            savedUser = user;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        savedUser.Should().NotBeNull();
        savedUser!.FirstName.Should().Be(command.FirstName);
        savedUser.LastName.Should().Be(command.LastName);
        savedUser.FullName.Should().Be($"{command.FirstName} {command.LastName}");
        savedUser.Email.Should().Be(command.Email);
        savedUser.Username.Should().Be(command.Username);
        savedUser.PasswordHash.Should().Be("hashed-password");
        savedUser.RoleId.Should().Be(command.RoleId);
        savedUser.DepartmentId.Should().Be(command.DepartmentId);
        savedUser.JobTitle.Should().Be(command.JobTitle);
        savedUser.IsActive.Should().BeTrue();

        result.UserId.Should().Be(42);
        result.FullName.Should().Be(savedUser.FullName);
        result.Email.Should().Be(command.Email);

        await repo.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "User"),
            Arg.Is<int>(value => value == 42),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(savedUser.FullName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenUsernameExists_ThrowsAndDoesNotAddUser()
    {
        // Arrange
        var repo = Substitute.For<IUserRepository>();
        var passwordService = Substitute.For<IPasswordService>();
        var audit = Substitute.For<IAuditService>();
        var handler = new CreateUserCommandHandler(repo, passwordService, audit);
        var command = CreateValidCommand();

        repo.UsernameExistsAsync(command.Username, null, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var act = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Username is already in use.");

        await repo.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    private static CreateUserCommand CreateValidCommand()
    {
        return new CreateUserCommand
        {
            FirstName = "Alex",
            LastName = "Morgan",
            Email = "alex.morgan@example.com",
            Username = "alex.morgan",
            Password = "Password123!",
            RoleId = 1,
            DepartmentId = 2,
            JobTitle = "Administrator",
            IsActive = true
        };
    }
}
