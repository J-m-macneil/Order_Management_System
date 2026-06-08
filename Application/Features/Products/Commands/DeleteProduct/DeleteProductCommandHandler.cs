using Application.Interfaces;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _repo;
    private readonly IAuditService _audit;

    public DeleteProductCommandHandler(
        IProductRepository repo,
        IAuditService audit)
    {
        _repo = repo;
        _audit = audit;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _repo.GetByIdAsync(request.ProductId, ct);

        if (product == null)
            return;

        var oldValues = new
        {
            product.ProductId,
            product.ProductName,
            product.IsActive,
            product.DeletedAt
        };

        product.IsActive = false;
        product.DeletedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(product, ct);

        await _audit.LogAsync(
            "Product",
            product.ProductId,
            "Deleted",
            oldValues,
            new
            {
                product.ProductId,
                product.ProductName,
                product.IsActive,
                product.DeletedAt
            },
            $"Product deleted: {product.ProductName}.",
            ct);
    }
}
