using Application.Common.Services;
using Application.Features.Products.Commands.UpdateProduct;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace Application.UnitTests.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingProduct_UpdatesProductAndWritesAuditLog()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateProductCommandHandler(repo, audit, new AuditChangeFormatter());
        var existingProduct = CreateExistingProduct();
        var request = CreateValidRequest();

        repo.GetByIdAsync(request.ProductId, Arg.Any<CancellationToken>())
            .Returns(existingProduct);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        existingProduct.ProductId.Should().Be(request.ProductId);
        existingProduct.SKU.Should().Be(request.Data.SKU);
        existingProduct.ProductName.Should().Be(request.Data.ProductName);
        existingProduct.Description.Should().Be(request.Data.Description);
        existingProduct.ProductCategoryId.Should().Be(request.Data.ProductCategoryId);
        existingProduct.UnitOfMeasureId.Should().Be(request.Data.UnitOfMeasureId);
        existingProduct.PackSize.Should().Be(request.Data.PackSize);
        existingProduct.BasePrice.Should().Be(request.Data.BasePrice);
        existingProduct.Currency.Should().Be(request.Data.Currency);
        existingProduct.HazardClassId.Should().Be(request.Data.HazardClassId);
        existingProduct.UNNumber.Should().Be(request.Data.UNNumber);
        existingProduct.StorageRequirement.Should().Be(request.Data.StorageRequirement);
        existingProduct.RequiresSds.Should().Be(request.Data.RequiresSds);
        existingProduct.IsRestricted.Should().Be(request.Data.IsRestricted);
        existingProduct.IsActive.Should().Be(request.Data.IsActive);

        await repo.Received(1)
            .GetByIdAsync(request.ProductId, Arg.Any<CancellationToken>());

        await repo.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        await audit.Received(1).LogAsync(
            Arg.Is<string>(value => value == "Product"),
            Arg.Is<int>(value => value == existingProduct.ProductId),
            Arg.Is<string>(value => value == "Updated"),
            Arg.Any<object>(),
            Arg.Any<object>(),
            Arg.Is<string>(message => message.Contains(request.Data.ProductName)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ThrowsExceptionAndDoesNotUpdateProduct()
    {
        // Arrange
        var repo = Substitute.For<IProductRepository>();
        var audit = Substitute.For<IAuditService>();

        var handler = new UpdateProductCommandHandler(repo, audit, new AuditChangeFormatter());
        var request = CreateValidRequest();

        repo.GetByIdAsync(request.ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        var act = () => handler.Handle(request, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Product not found");

        await repo.Received(1)
            .GetByIdAsync(request.ProductId, Arg.Any<CancellationToken>());

        await repo.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());

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
            SKU = "OLD-001",
            ProductName = "Old Product",
            Description = "Old description",
            ProductCategoryId = 1,
            UnitOfMeasureId = 1,
            PackSize = "10L",
            BasePrice = 20m,
            Currency = "GBP",
            HazardClassId = 1,
            UNNumber = "UN0001",
            StorageRequirement = "Old storage",
            RequiresSds = false,
            IsRestricted = false,
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }

    private static UpdateProductRequest CreateValidRequest()
    {
        return new UpdateProductRequest
        {
            ProductId = 123,
            Data = new UpdateProductCommand
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
            }
        };
    }
}
