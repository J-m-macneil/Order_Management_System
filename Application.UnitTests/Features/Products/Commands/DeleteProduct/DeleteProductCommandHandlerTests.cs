using Application.Features.Products.Commands.DeleteProduct;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_SoftDeletesProductAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteProductCommandHandler(repo, audit);
        var product = CreateExistingProduct();
        var command = new DeleteProductCommand { ProductId = product.ProductId };

        repo.GetByIdAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns(product);

        // Act
        var before = DateTime.UtcNow;
        await handler.Handle(command, CancellationToken.None);
        var after = DateTime.UtcNow;

        // Assert
        product.IsActive.Should().BeFalse();
        product.DeletedAt.Should().NotBeNull();
        product.DeletedAt!.Value.Should().BeOnOrAfter(before);
        product.DeletedAt.Value.Should().BeOnOrBefore(after);

        await repo.Received(1)
            .GetByIdAsync(command.ProductId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .UpdateAsync(product, Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Product"),
            Arg.Is<int>(value => value == product.ProductId),
            Arg.Is<string>(value => value == "Deleted"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(product.ProductName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_DoesNotUpdateProductOrWriteAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new DeleteProductCommandHandler(repo, audit);
        var command = new DeleteProductCommand { ProductId = 123 };

        repo.GetByIdAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repo.Received(1)
            .GetByIdAsync(command.ProductId, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await audit.DidNotReceive().LogAsync(
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<object?>(),
            Arg.Any<object?>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    private static Product CreateExistingProduct()
    {
        return new Product
        {
            ProductId = 123,
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
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1),
            DeletedAt = null
        };
    }
}
