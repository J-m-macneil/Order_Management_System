using Application.Features.Customers.Queries.GetCustomerSummary;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Customers.Queries.GetCustomerSummary;

public class GetCustomerSummaryQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCustomerSummary()
    {
        // Arrange
        var repo = Substitute.For<ICustomerRepository>();
        var handler = new GetCustomerSummaryQueryHandler(repo);
        var query = new GetCustomerSummaryQuery();

        repo.GetSummaryAsync(Arg.Any<CancellationToken>())
            .Returns((TotalCustomers: 10, ActiveCustomers: 7, InactiveCustomers: 3));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalCustomers.Should().Be(10);
        result.ActiveCustomers.Should().Be(7);
        result.InactiveCustomers.Should().Be(3);

        await repo.Received(1)
            .GetSummaryAsync(Arg.Any<CancellationToken>());
    }
}
