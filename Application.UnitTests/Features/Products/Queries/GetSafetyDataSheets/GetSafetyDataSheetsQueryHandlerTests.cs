using Application.Features.Products.Queries.GetSafetyDataSheets;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Queries.GetSafetyDataSheets;

public class GetSafetyDataSheetsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithProductSafetyDataSheets_ReturnsDtos()
    {
        // Arrange
        var repo = Substitute.For<ISafetyDataSheetRepository>();
        var handler = new GetSafetyDataSheetsQueryHandler(repo);
        var query = new GetSafetyDataSheetsQuery { ProductId = 123 };
        var items = new List<SafetyDataSheet>
        {
            CreateSafetyDataSheet()
        };

        repo.GetByProductIdAsync(query.ProductId, Arg.Any<CancellationToken>())
            .Returns(items);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().ContainSingle();

        var item = result.Single();
        item.SafetyDataSheetId.Should().Be(items[0].SafetyDataSheetId);
        item.ProductId.Should().Be(items[0].ProductId);
        item.FileName.Should().Be(items[0].FileName);
        item.FilePath.Should().Be(items[0].FilePath);
        item.Version.Should().Be(items[0].Version);
        item.EffectiveDate.Should().Be(items[0].EffectiveDate);
        item.UploadedAt.Should().Be(items[0].UploadedAt);
        item.UploadedByUserId.Should().Be(items[0].UploadedByUserId);

        await repo.Received(1)
            .GetByProductIdAsync(query.ProductId, Arg.Any<CancellationToken>());
    }

    private static SafetyDataSheet CreateSafetyDataSheet()
    {
        return new SafetyDataSheet
        {
            SafetyDataSheetId = 456,
            ProductId = 123,
            FileName = "acetone-sds.pdf",
            FilePath = "/sds/acetone-sds.pdf",
            Version = "1.0",
            EffectiveDate = new DateTime(2026, 1, 1),
            UploadedAt = new DateTime(2026, 1, 2),
            UploadedByUserId = 4,
            IsActive = true
        };
    }
}
