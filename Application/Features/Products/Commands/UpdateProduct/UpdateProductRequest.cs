using MediatR;
using Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductRequest : IRequest<Unit>
{
    public int ProductId { get; set; }
    public UpdateProductCommand Data { get; set; } = null!;
}