using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Documents;
using Domain.Entities.Orders;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Infrastructure.Services;

public class OrderDocumentGenerator : IOrderDocumentGenerator
{
    private readonly AppDbContext _dbContext;

    public OrderDocumentGenerator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
            .FirstAsync(o => o.OrderId == orderId, cancellationToken);

        var safeOrderNumber = order.OrderNumber.ToLowerInvariant();
        var safeDocumentType = documentType.ToLowerInvariant();

        var fileName = $"{safeOrderNumber}_{safeDocumentType}.pdf";

        var documentsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "documents");

        Directory.CreateDirectory(documentsFolder);

        var physicalPath = Path.Combine(documentsFolder, fileName);

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text(documentType)
                        .FontSize(22)
                        .SemiBold();

                    column.Item().Text(order.OrderNumber)
                        .FontSize(12)
                        .FontColor(Colors.Grey.Darken2);
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(12);

                    AddOrderSummary(column, order);

                    if (documentType == DocumentType.SafetyDataSheetBundle)
                    {
                        AddSdsBundle(column, order);
                    }

                    AddOrderItems(column, order);
                    AddTotals(column, order);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated ");
                    text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
                    text.Span(" UTC");
                });
            });
        })
        .GeneratePdf(physicalPath);

        var document = new Document
        {
            OrderId = order.OrderId,
            DocumentType = documentType,
            FileName = fileName,
            FilePath = $"/documents/{fileName}",
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = null
        };

        _dbContext.Documents.Add(document);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return document;
    }

    private static void AddOrderSummary(ColumnDescriptor column, Order order)
    {
        column.Item().Text("Order Details").FontSize(14).SemiBold();

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            AddRow(table, "Customer", order.Customer?.CompanyName ?? $"Customer #{order.CustomerId}");
            AddRow(table, "Warehouse", order.Warehouse?.Name ?? "—");
            AddRow(table, "Carrier", order.Carrier?.Name ?? "—");
            AddRow(table, "Created", order.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
            AddRow(table, "Requested Delivery", order.RequestedDeliveryDate.ToString("yyyy-MM-dd") ?? "—");
            AddRow(table, "Priority", order.IsPriorityOrder ? "Yes" : "No");
        });
    }

    private static void AddSdsBundle(ColumnDescriptor column, Order order)
    {
        column.Item().Text("Safety Data Sheet Bundle").FontSize(14).SemiBold();

        column.Item().Text(
            "This simulated SDS bundle confirms that hazardous or restricted product documentation has been generated for operational review.");
    }

    private static void AddOrderItems(ColumnDescriptor column, Order order)
    {
        column.Item().Text("Items").FontSize(14).SemiBold();

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Text("Product").SemiBold();
                header.Cell().AlignRight().Text("Qty").SemiBold();
                header.Cell().AlignRight().Text("Unit").SemiBold();
                header.Cell().AlignRight().Text("Line Total").SemiBold();
            });

            foreach (var item in order.OrderItems)
            {
                table.Cell().Text(item.Product?.ProductName ?? $"Product #{item.ProductId}");
                table.Cell().AlignRight().Text(item.Quantity.ToString());
                table.Cell().AlignRight().Text(item.UnitPrice.ToString("0.00"));
                table.Cell().AlignRight().Text(item.LineTotal.ToString("0.00"));
            }
        });
    }

    private static void AddTotals(ColumnDescriptor column, Order order)
    {
        column.Item().AlignRight().Column(totals =>
        {
            totals.Item().Text($"Subtotal: {order.Subtotal:0.00} {order.Currency}");
            totals.Item().Text($"Discount: {order.DiscountAmount:0.00} {order.Currency}");
            totals.Item().Text($"Tax: {order.TaxAmount:0.00} {order.Currency}");
            totals.Item().Text($"Total: {order.TotalAmount:0.00} {order.Currency}")
                .FontSize(14)
                .SemiBold();
        });
    }

    private static void AddRow(TableDescriptor table, string label, string value)
    {
        table.Cell().Text(label).SemiBold();
        table.Cell().Text(value);
    }
}
