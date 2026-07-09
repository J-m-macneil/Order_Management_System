using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities.Orders;

namespace Infrastructure.Services.Documents;

internal class SdsDocumentFileResolver
{
    private readonly IFileStorageService _fileStorage;

    public SdsDocumentFileResolver(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public IReadOnlyCollection<string> GetRequiredFileKeys(Order order)
    {
        var fileKeys = new List<string>();
        var missingFiles = new List<string>();

        foreach (var item in GetSdsOrderItems(order))
        {
            var product = item.Product;
            var activeSds = product.SafetyDataSheets
                .Where(s => s.IsActive && s.DeletedAt == null)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefault();

            if (activeSds == null)
            {
                missingFiles.Add($"{product.ProductName} ({product.SKU})");
                continue;
            }

            if (string.IsNullOrWhiteSpace(activeSds.FilePath) ||
                !_fileStorage.FileExists(activeSds.FilePath))
            {
                missingFiles.Add($"{product.ProductName} ({product.SKU}) - {activeSds.FileName}");
                continue;
            }

            fileKeys.Add(activeSds.FilePath);
        }

        if (missingFiles.Count > 0)
        {
            throw new OperatorActionRequiredException(
                $"Cannot generate SDS bundle. SDS PDF file missing for: {string.Join(", ", missingFiles)}.");
        }

        return fileKeys
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static List<OrderItem> GetSdsOrderItems(Order order)
    {
        return order.OrderItems
            .Where(i => i.DeletedAt == null && i.Product.RequiresSds)
            .GroupBy(i => i.ProductId)
            .Select(g => g.First())
            .ToList();
    }
}
