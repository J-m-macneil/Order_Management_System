using Application.Features.Products.Queries.GetProductSummary;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Queries.GetProductSummary;

public class GetProductSummaryQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithSummaryData_ReturnsProductSummary()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var handler = new GetProductSummaryQueryHandler(repo);
        var query = new GetProductSummaryQuery();

        repo.GetSummaryAsync(Arg.Any<CancellationToken>())
            .Returns((12, 10, 3, 4));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.TotalProducts.Should().Be(12);
        result.ActiveProducts.Should().Be(10);
        result.RestrictedProducts.Should().Be(3);
        result.HazardousProducts.Should().Be(4);

        await repo.Received(1)
            .GetSummaryAsync(Arg.Any<CancellationToken>());
    }
}
