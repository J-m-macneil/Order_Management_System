using Application.Features.Products.Queries.GetUnitOfMeasures;
using Domain.Entities.Products;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Queries.GetUnitOfMeasures;

public class GetUnitOfMeasuresQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithUnitOfMeasures_ReturnsDtos()
    {
        // Arrange
        var repo = Substitute.For<IUnitOfMeasureRepository>();
        var handler = new GetUnitOfMeasuresQueryHandler(repo);
        var query = new GetUnitOfMeasuresQuery();
        var units = new List<UnitOfMeasure>
        {
            new()
            {
                UnitOfMeasureId = 1,
                Code = "L",
                Name = "Litre"
            },
            new()
            {
                UnitOfMeasureId = 2,
                Code = "KG",
                Name = "Kilogram"
            }
        };

        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(units);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        result[0].UnitOfMeasureId.Should().Be(units[0].UnitOfMeasureId);
        result[0].Code.Should().Be(units[0].Code);
        result[0].Name.Should().Be(units[0].Name);

        result[1].UnitOfMeasureId.Should().Be(units[1].UnitOfMeasureId);
        result[1].Code.Should().Be(units[1].Code);
        result[1].Name.Should().Be(units[1].Name);

        await repo.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }
}
