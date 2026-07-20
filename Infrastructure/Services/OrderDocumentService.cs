using Application.Interfaces;

namespace Infrastructure.Services;

public class OrderDocumentService : IOrderDocumentService
{
    private readonly IOrderDocumentGenerator _documentGenerator;
    private readonly IAuditService _auditService;

    public OrderDocumentService(
        IOrderDocumentGenerator documentGenerator,
        IAuditService auditService)
    {
        _documentGenerator = documentGenerator;
        _auditService = auditService;
    }

    public async Task GenerateAsync(
        int orderId,
        string documentType,
        CancellationToken cancellationToken)
    {
        var document = await _documentGenerator.GenerateAsync(
            orderId,
            documentType,
            cancellationToken);

        _auditService.AddSystemAction(
            "Document",
            document.DocumentId,
            "Generated",
            null,
            new
            {
                document.DocumentId,
                orderId,
                DocumentType = documentType
            },
            $"{documentType} generated as PDF by background job.");
    }
}
