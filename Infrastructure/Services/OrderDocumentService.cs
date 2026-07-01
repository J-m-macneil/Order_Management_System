using Application.Interfaces;
using Domain.Entities.Documents;
using Domain.Entities.Orders;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderDocumentService : IOrderDocumentService
{
    private readonly AppDbContext _dbContext;
    private readonly IOrderDocumentGenerator _documentGenerator;
    private readonly IAuditService _auditService;

    public OrderDocumentService(
        AppDbContext dbContext,
        IOrderDocumentGenerator documentGenerator,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _documentGenerator = documentGenerator;
        _auditService = auditService;
    }

    public async Task GenerateForJobAsync(
        ProcessingJob job,
        CancellationToken cancellationToken)
    {
        var documentType = GetDocumentType(job.JobType);
        var document = await _documentGenerator.GenerateAsync(
            job.OrderId,
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
                job.OrderId,
                DocumentType = documentType
            },
            $"{documentType} generated as PDF by background job.");
    }

    public async Task<bool> RequiredApprovalDocumentsExistAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var requiresSdsBundle = await OrderRequiresSdsBundleAsync(orderId, cancellationToken);
        var requiredDocumentTypes = GetRequiredApprovalDocumentTypes(requiresSdsBundle);

        var generatedDocumentTypes = await _dbContext.Documents
            .Where(d => d.OrderId == orderId)
            .Where(d => requiredDocumentTypes.Contains(d.DocumentType))
            .Select(d => d.DocumentType)
            .Distinct()
            .ToListAsync(cancellationToken);

        return requiredDocumentTypes.All(generatedDocumentTypes.Contains);
    }

    public async Task<IReadOnlyCollection<string>> GetMissingApprovalDocumentTypesAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        var requiredDocumentTypes = GetRequiredApprovalDocumentTypes(order);

        var generatedDocumentTypes = await _dbContext.Documents
            .Where(d => d.OrderId == order.OrderId)
            .Where(d => requiredDocumentTypes.Contains(d.DocumentType))
            .Select(d => d.DocumentType)
            .Distinct()
            .ToListAsync(cancellationToken);

        return requiredDocumentTypes
            .Except(generatedDocumentTypes)
            .ToList();
    }

    public string GetGenerationJobType(string documentType)
    {
        return documentType switch
        {
            DocumentType.OrderSummary => ProcessingJobType.GenerateOrderSummaryDocument,
            DocumentType.SafetyDataSheetBundle => ProcessingJobType.GenerateSdsBundle,
            _ => throw new InvalidOperationException($"No job type configured for document type '{documentType}'.")
        };
    }

    private static string GetDocumentType(string jobType)
    {
        return jobType switch
        {
            ProcessingJobType.GenerateOrderSummaryDocument => DocumentType.OrderSummary,
            ProcessingJobType.GenerateSdsBundle => DocumentType.SafetyDataSheetBundle,
            _ => throw new InvalidOperationException($"Job type '{jobType}' does not generate a document.")
        };
    }

    private static List<string> GetRequiredApprovalDocumentTypes(Order order)
    {
        return GetRequiredApprovalDocumentTypes(OrderRequiresSdsBundle(order));
    }

    private static List<string> GetRequiredApprovalDocumentTypes(bool requiresSdsBundle)
    {
        var documentTypes = new List<string> { DocumentType.OrderSummary };

        if (requiresSdsBundle)
        {
            documentTypes.Add(DocumentType.SafetyDataSheetBundle);
        }

        return documentTypes;
    }

    private static bool OrderRequiresSdsBundle(Order order)
    {
        return order.OrderItems.Any(i =>
            i.DeletedAt == null &&
            i.Product.RequiresSds);
    }

    private async Task<bool> OrderRequiresSdsBundleAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.OrderItems
            .Where(i => i.OrderId == orderId && i.DeletedAt == null)
            .AnyAsync(i => i.Product.RequiresSds, cancellationToken);
    }

}
