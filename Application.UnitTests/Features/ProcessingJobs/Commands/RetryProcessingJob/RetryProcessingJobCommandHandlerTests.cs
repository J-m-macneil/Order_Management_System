using Application.Common.Services;
using Application.Features.ProcessingJobs.Commands.RetryProcessingJob;
using Application.Interfaces;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.ProcessingJobs.Commands.RetryProcessingJob;

public class RetryProcessingJobCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithFailedJob_RequeuesJobResetsAttemptCycleAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IProcessingJobRepository>();
        var audit = Substitute.For<IAuditService>();
        var handler = new RetryProcessingJobCommandHandler(repo, audit, new AuditChangeFormatter());
        var job = CreateFailedJob();

        repo.GetByIdAsync(job.ProcessingJobId, Arg.Any<CancellationToken>())
            .Returns(job);

        // Act
        await handler.Handle(new RetryProcessingJobCommand { Id = job.ProcessingJobId }, CancellationToken.None);

        // Assert
        job.Status.Should().Be("Queued");
        job.AttemptCount.Should().Be(0);
        job.ErrorMessage.Should().BeNull();
        job.FailedAt.Should().BeNull();
        job.LastRetryAt.Should().NotBeNull();
        job.NextAttemptAt.Should().NotBeNull();

        await repo.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "ProcessingJob"),
            Arg.Is<int>(value => value == job.ProcessingJobId),
            Arg.Is<string>(value => value == "RetryQueued"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(value => value.Contains(job.JobType)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenJobIsNotFailed_ThrowsAndDoesNotSave()
    {
        // Arrange
        var repo = Substitute.For<IProcessingJobRepository>();
        var audit = Substitute.For<IAuditService>();
        var handler = new RetryProcessingJobCommandHandler(repo, audit, new AuditChangeFormatter());
        var job = CreateFailedJob();
        job.Status = "Completed";

        repo.GetByIdAsync(job.ProcessingJobId, Arg.Any<CancellationToken>())
            .Returns(job);

        // Act
        var act = () => handler.Handle(new RetryProcessingJobCommand { Id = job.ProcessingJobId }, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Only failed jobs can be retried.");

        await repo.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static ProcessingJob CreateFailedJob()
    {
        return new ProcessingJob
        {
            ProcessingJobId = 44,
            OrderId = 123,
            JobType = "GenerateSdsBundle",
            Status = "Failed",
            AttemptCount = 3,
            MaxAttempts = 3,
            FailedAt = new DateTime(2026, 6, 1, 12, 0, 0),
            ErrorMessage = "Template missing"
        };
    }
}
