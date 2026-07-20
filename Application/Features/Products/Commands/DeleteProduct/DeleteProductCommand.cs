using MediatR;

namespace Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest
{
    public int ProductId { get; set; }
}
