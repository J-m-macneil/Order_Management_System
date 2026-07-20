using Application.Common.Services;
using FluentAssertions;

namespace Application.UnitTests.Common.Services;

public class AuditChangeFormatterTests
{
    private readonly AuditChangeFormatter _formatter = new();

    [Fact]
    public void CreateChangeSummary_WhenDecimalValuesAreEquivalent_ReturnsNull()
    {
        // Arrange
        const string oldValuesJson = """{"basePrice":30.00}""";
        const string newValuesJson = """{"basePrice":30}""";

        // Act
        var result = _formatter.CreateChangeSummary(oldValuesJson, newValuesJson);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CreateChangeSummary_WhenJsonNumberAndNumericStringAreEquivalent_ReturnsNull()
    {
        // Arrange
        const string oldValuesJson = """{"basePrice":30.00}""";
        const string newValuesJson = """{"basePrice":"30"}""";

        // Act
        var result = _formatter.CreateChangeSummary(oldValuesJson, newValuesJson);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CreateChangeSummary_WhenBooleanValueChanges_ReturnsReadableSummary()
    {
        // Arrange
        const string oldValuesJson = """{"requiresSds":true}""";
        const string newValuesJson = """{"requiresSds":false}""";

        // Act
        var result = _formatter.CreateChangeSummary(oldValuesJson, newValuesJson);

        // Assert
        result.Should().Be("SDS required changed from Yes to No");
    }

    [Fact]
    public void CreateChangeSummary_WhenMultipleValuesChange_ReturnsReadableSummary()
    {
        // Arrange
        const string oldValuesJson = """{"basePrice":30,"isRestricted":false,"requiresSds":true}""";
        const string newValuesJson = """{"basePrice":35,"isRestricted":true,"requiresSds":false}""";

        // Act
        var result = _formatter.CreateChangeSummary(oldValuesJson, newValuesJson);

        // Assert
        result.Should().Be("Base price changed from 30 to 35; Restricted changed from No to Yes; SDS required changed from Yes to No");
    }

    [Fact]
    public void CreateChangeSummary_WhenInvalidJson_ReturnsNull()
    {
        // Act
        var result = _formatter.CreateChangeSummary("{not-json", """{"basePrice":30}""");

        // Assert
        result.Should().BeNull();
    }
}
