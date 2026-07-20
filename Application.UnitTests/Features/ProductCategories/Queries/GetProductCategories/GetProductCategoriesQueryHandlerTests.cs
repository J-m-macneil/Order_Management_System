using Application.Features.ProductCategories.Queries.GetProductCategories;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.ProductCategories.Queries.GetProductCategories;

public class GetProductCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithProductCategories_ReturnsDtos()
    {
        // Arrange
        var repo = Substitute.For<IProductCategoryRepository>();
        var handler = new GetProductCategoriesQueryHandler(repo);
        var query = new GetProductCategoriesQuery();
        var categories = new List<ProductCategory>
        {
            new()
            {
                ProductCategoryId = 1,
                Name = "Solvents"
            },
            new()
            {
                ProductCategoryId = 2,
                Name = "Cleaning"
            }
        };

        repo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        result[0].ProductCategoryId.Should().Be(categories[0].ProductCategoryId);
        result[0].Name.Should().Be(categories[0].Name);

        result[1].ProductCategoryId.Should().Be(categories[1].ProductCategoryId);
        result[1].Name.Should().Be(categories[1].Name);

        await repo.Received(1)
            .GetAllAsync(Arg.Any<CancellationToken>());
    }
}
