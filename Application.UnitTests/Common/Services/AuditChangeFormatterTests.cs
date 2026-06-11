using Application.Common.Services;
using FluentAssertions;

namespace Application.UnitTests.Common.Services;

public class AuditChangeFormatterTests
{
    private readonly AuditChangeFormatter _formatter = new();

    [Fact]
    public void GetChanges_WhenValuesAreEquivalentDecimals_DoesNotReturnChange()
    {
        // Arrange
        var oldValues = new { BasePrice = 30.00m };
        var newValues = new { BasePrice = 30m };

        // Act
        var result = _formatter.GetChanges(oldValues, newValues);

        // Assert
        result.Should().BeEmpty();
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
    public void GetChanges_WhenBooleanValueChanges_ReturnsReadableChange()
    {
        // Arrange
        var oldValues = new { RequiresSds = true };
        var newValues = new { RequiresSds = false };

        // Act
        var result = _formatter.GetChanges(oldValues, newValues);

        // Assert
        result.Should().ContainSingle();
        result[0].FieldName.Should().Be("requiresSds");
        result[0].DisplayName.Should().Be("SDS required");
        result[0].OldValue.Should().Be("Yes");
        result[0].NewValue.Should().Be("No");
    }

    [Fact]
    public void CreateChangeSummary_WhenMultipleValuesChange_ReturnsReadableSummary()
    {
        // Arrange
        var oldValues = new
        {
            BasePrice = 30m,
            IsRestricted = false,
            RequiresSds = true
        };

        var newValues = new
        {
            BasePrice = 35m,
            IsRestricted = true,
            RequiresSds = false
        };

        var changes = _formatter.GetChanges(oldValues, newValues);

        // Act
        var result = _formatter.CreateChangeSummary(changes);

        // Assert
        result.Should().Be("Base price changed from 30 to 35; Restricted changed from No to Yes; SDS required changed from Yes to No");
    }

    [Fact]
    public void CreateUpdateNote_WhenValuesChange_ReturnsEntityNoteWithReadableSummary()
    {
        // Arrange
        var oldValues = new
        {
            CompanyName = "Old Customer",
            CreditLimit = 1000m
        };

        var newValues = new
        {
            CompanyName = "New Customer",
            CreditLimit = 2500m
        };

        var changes = _formatter.GetChanges(oldValues, newValues);

        // Act
        var result = _formatter.CreateUpdateNote("Customer", "New Customer", changes);

        // Assert
        result.Should().Be("Customer updated: New Customer; company name changed from Old Customer to New Customer; credit limit changed from 1000 to 2500.");
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
