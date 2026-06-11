using Application.Features.Products.Commands.CreateProduct;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_AddsProductAndReturnsDto()
    {
        // Arrange
        Product? savedProduct = null;

        var repo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new CreateProductCommandHandler(repo, audit);
        var command = CreateValidCommand();

        repo.AddAsync(Arg.Do<Product>(product =>
        {
            product.ProductId = 123;
            savedProduct = product;
        }), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

        // Act
        var before = DateTime.UtcNow;
        var result = await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        savedProduct.Should().NotBeNull();

        savedProduct!.ProductId.Should().Be(123);
        savedProduct.SKU.Should().Be(command.SKU);
        savedProduct.ProductName.Should().Be(command.ProductName);
        savedProduct.Description.Should().Be(command.Description);
        savedProduct.ProductCategoryId.Should().Be(command.ProductCategoryId);
        savedProduct.UnitOfMeasureId.Should().Be(command.UnitOfMeasureId);
        savedProduct.PackSize.Should().Be(command.PackSize);
        savedProduct.BasePrice.Should().Be(command.BasePrice);
        savedProduct.Currency.Should().Be(command.Currency);
        savedProduct.HazardClassId.Should().Be(command.HazardClassId);
        savedProduct.UNNumber.Should().Be(command.UNNumber);
        savedProduct.StorageRequirement.Should().Be(command.StorageRequirement);
        savedProduct.RequiresSds.Should().Be(command.RequiresSds);
        savedProduct.IsRestricted.Should().Be(command.IsRestricted);
        savedProduct.IsActive.Should().Be(command.IsActive);
        savedProduct.CreatedAt.Should().BeOnOrAfter(before);
        savedProduct.CreatedAt.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .AddAsync(savedProduct, Arg.Any<CancellationToken>());

        result.ProductId.Should().Be(savedProduct.ProductId);
        result.SKU.Should().Be(command.SKU);
        result.ProductName.Should().Be(command.ProductName);
        result.Description.Should().Be(command.Description);
        result.ProductCategoryId.Should().Be(command.ProductCategoryId);
        result.UnitOfMeasureId.Should().Be(command.UnitOfMeasureId);
        result.PackSize.Should().Be(command.PackSize);
        result.BasePrice.Should().Be(command.BasePrice);
        result.Currency.Should().Be(command.Currency);
        result.HazardClassId.Should().Be(command.HazardClassId);
        result.UNNumber.Should().Be(command.UNNumber);
        result.StorageRequirement.Should().Be(command.StorageRequirement);
        result.RequiresSds.Should().Be(command.RequiresSds);
        result.IsRestricted.Should().Be(command.IsRestricted);
        result.IsActive.Should().Be(command.IsActive);

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Product"),
            Arg.Is<int>(value => value == savedProduct.ProductId),
            Arg.Is<string>(value => value == "Created"),
            Arg.Is<object?>(value => value == null),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(command.ProductName)),
            Arg.Any<CancellationToken>());
    }

    private static CreateProductCommand CreateValidCommand()
    {
        return new CreateProductCommand
        {
            SKU = "CHEM-001",
            ProductName = "Acetone",
            Description = "Industrial solvent",
            ProductCategoryId = 2,
            UnitOfMeasureId = 3,
            PackSize = "25L",
            BasePrice = 29.50m,
            Currency = "GBP",
            HazardClassId = 4,
            UNNumber = "UN1090",
            StorageRequirement = "Store in a flammable cabinet",
            RequiresSds = true,
            IsRestricted = true,
            IsActive = true
        };
    }
}
