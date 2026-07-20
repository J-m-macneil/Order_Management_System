using Application.Common.Interfaces;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Documents;
using Infrastructure.Persistence.Context;
using Infrastructure.Services.Documents;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderDocumentGenerator : IOrderDocumentGenerator
{
    private readonly AppDbContext _dbContext;
    private readonly IFileStorageService _fileStorage;
    private readonly OrderDocumentPdfBuilder _pdfBuilder;
    private readonly SdsDocumentFileResolver _sdsFileResolver;

    public OrderDocumentGenerator(
        AppDbContext dbContext,
        IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
        _pdfBuilder = new OrderDocumentPdfBuilder();
        _sdsFileResolver = new SdsDocumentFileResolver(fileStorage);
    }

    public async Task<Document> GenerateAsync(
        int orderId,
        string documentType,
        CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Warehouse)
            .Include(o => o.Carrier)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.HazardClass)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.SafetyDataSheets)
            .FirstAsync(o => o.OrderId == orderId, cancellationToken);

        var sdsFileKeys = documentType == DocumentType.SafetyDataSheetBundle
            ? _sdsFileResolver.GetRequiredFileKeys(order)
            : Array.Empty<string>();

        var pdfBytes = _pdfBuilder.Build(order, documentType);

        if (sdsFileKeys.Count > 0)
        {
            var sdsPdfs = new List<byte[]>();

            foreach (var sdsFileKey in sdsFileKeys)
            {
                sdsPdfs.Add(await _fileStorage.GetFileAsync(sdsFileKey, cancellationToken));
            }

            pdfBytes = PdfMergeService.Merge(pdfBytes, sdsPdfs);
        }

        var fileName = CreateFileName(order.OrderNumber, documentType);
        var fileKey = $"orders/{fileName}";

        await _fileStorage.SaveFileAsync(fileKey, pdfBytes, cancellationToken);

        var document = new Document
        {
            OrderId = order.OrderId,
            DocumentType = documentType,
            FileName = fileName,
            FilePath = fileKey,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = null
        };

        _dbContext.Documents.Add(document);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }

    private static string CreateFileName(string orderNumber, string documentType)
    {
        var safeOrderNumber = orderNumber.ToLowerInvariant();
        var safeDocumentType = documentType.ToLowerInvariant();

        return $"{safeOrderNumber}_{safeDocumentType}.pdf";
    }
}
