namespace Application.Interfaces;

public interface IOrderDocumentService
{
    Task GenerateAsync(
        int orderId,
        string documentType,
        CancellationToken cancellationToken);
}
