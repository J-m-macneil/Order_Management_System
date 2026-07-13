using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

public class SafetyDataSheetDocumentGenerator : ISafetyDataSheetDocumentGenerator
{
    private const string BrandBlue = "#1CA8FE";
    private const string Ink = "#111827";
    private const string Muted = "#64748B";
    private const string Border = "#D7DEE8";
    private const string Surface = "#F8FAFC";
    private const string DocumentLogoPath = "Assets/login-logo-light.svg";
    private static readonly CultureInfo UkCulture = CultureInfo.GetCultureInfo("en-GB");

    private readonly AppDbContext _dbContext;
    private readonly IAuditService _auditService;
    private readonly IFileStorageService _fileStorage;

    public SafetyDataSheetDocumentGenerator(
        AppDbContext dbContext,
        IAuditService auditService,
        IFileStorageService fileStorage)
    {
        _dbContext = dbContext;
        _auditService = auditService;
        _fileStorage = fileStorage;
    }

    public async Task<SafetyDataSheetDto> GenerateAsync(
        int productId,
        int generatedByUserId,
        CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products
            .Include(p => p.HazardClass)
            .Include(p => p.SafetyDataSheets)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.DeletedAt == null, cancellationToken)
            ?? throw new NotFoundException("Product", productId);

        if (!product.RequiresSds)
        {
            throw new BadRequestException("This product does not require an SDS.");
        }

        foreach (var activeSds in product.SafetyDataSheets.Where(s => s.IsActive && s.DeletedAt == null))
        {
            activeSds.IsActive = false;
        }

        var version = $"{product.SafetyDataSheets.Count(s => s.DeletedAt == null) + 1}.0";
        var now = DateTime.UtcNow;
        var fileName = $"{CreateSafeFileName(product.SKU)}_sds_v{version.Replace(".", "_")}.pdf";
        var fileKey = $"sds/{fileName}";

        var fileBytes = GeneratePdf(product, version, now);
        await _fileStorage.SaveFileAsync(fileKey, fileBytes, cancellationToken);

        var safetyDataSheet = new SafetyDataSheet
        {
            ProductId = product.ProductId,
            FileName = fileName,
            FilePath = fileKey,
            Version = version,
            EffectiveDate = now.Date,
            UploadedAt = now,
            UploadedByUserId = generatedByUserId,
            IsActive = true
        };

        _dbContext.SafetyDataSheets.Add(safetyDataSheet);

        _auditService.AddUserAction(
            "Product",
            product.ProductId,
            "Generated",
            oldValues: null,
            newValues: new
            {
                product.ProductId,
                product.SKU,
                product.ProductName,
                safetyDataSheet.FileName,
                safetyDataSheet.Version
            },
            notes: $"Generated SDS {version} for product #{product.ProductId}: {product.ProductName}.");

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SafetyDataSheetDto
        {
            SafetyDataSheetId = safetyDataSheet.SafetyDataSheetId,
            ProductId = safetyDataSheet.ProductId,
            FileName = safetyDataSheet.FileName,
            FilePath = safetyDataSheet.FilePath,
            Version = safetyDataSheet.Version,
            EffectiveDate = safetyDataSheet.EffectiveDate,
            UploadedAt = safetyDataSheet.UploadedAt,
            UploadedByUserId = safetyDataSheet.UploadedByUserId,
            UploadedByUserName = null,
            IsActive = safetyDataSheet.IsActive
        };
    }

    private static byte[] GeneratePdf(Product product, string version, DateTime generatedAt)
    {
        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Ink));

                page.Header().Element(header => AddHeader(header, product, version));

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(16);

                    AddSection(column, "Product Identification", table =>
                    {
                        AddRow(table, "Product", product.ProductName);
                        AddRow(table, "SKU", product.SKU);
                        AddRow(table, "Hazard Class", product.HazardClass?.Name ?? "Not specified");
                        AddRow(table, "UN Number", string.IsNullOrWhiteSpace(product.UNNumber) ? "Not specified" : product.UNNumber);
                        AddRow(table, "Restricted", product.IsRestricted ? "Yes" : "No");
                    });

                    AddSection(column, "Handling And Storage", table =>
                    {
                        AddRow(table, "Storage Requirement", string.IsNullOrWhiteSpace(product.StorageRequirement) ? "Not specified" : product.StorageRequirement);
                        AddRow(table, "Handling Guidance", "Use appropriate handling controls for the stated hazard class. Avoid uncontrolled release and follow site procedures.");
                        AddRow(table, "PPE", "Use task-appropriate gloves, eye protection and protective clothing according to site risk assessment.");
                    });

                    AddSection(column, "Emergency Information", table =>
                    {
                        AddRow(table, "Spill Response", "Contain safely where trained to do so. Prevent entry to drains and notify the site supervisor.");
                        AddRow(table, "First Aid", "Move affected person away from exposure and seek medical advice if symptoms persist.");
                        AddRow(table, "Transport Notes", "Check carrier requirements and product UN classification before dispatch.");
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Muted));
                    text.Span("Generated by Back. on ");
                    text.Span(generatedAt.ToString("dd MMM yyyy, HH:mm", UkCulture));
                    text.Span(" UTC");
                });
            });
        })
        .GeneratePdf();
    }

    private static void AddHeader(IContainer container, Product product, string version)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    var logoSvg = LoadDocumentLogoSvg();

                    if (logoSvg is not null)
                    {
                        left.Item().Width(150).Svg(logoSvg);
                    }
                    else
                    {
                        left.Item().Text(text =>
                        {
                            text.Span("Back").FontSize(28).Bold().FontColor(Colors.Black);
                            text.Span(".").FontSize(28).Bold().FontColor(BrandBlue);
                        });
                    }
                });

                row.ConstantItem(230).AlignRight().Column(right =>
                {
                    right.Item().AlignRight().Text("Safety Data Sheet")
                        .FontSize(18)
                        .SemiBold()
                        .FontColor(Ink);

                    right.Item().AlignRight().PaddingTop(4).Text($"{product.SKU} · Version {version}")
                        .FontSize(11)
                        .FontColor(Muted);
                });
            });

            column.Item().PaddingTop(18).Height(2).Background(BrandBlue);
        });
    }

    private static void AddSection(ColumnDescriptor column, string title, Action<TableDescriptor> addRows)
    {
        column.Item().Text(title)
            .FontSize(13)
            .SemiBold()
            .FontColor(Ink);

        column.Item().Border(1).BorderColor(Border).Background(Surface).Padding(14).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(140);
                columns.RelativeColumn();
            });

            addRows(table);
        });
    }

    private static void AddRow(TableDescriptor table, string label, string value)
    {
        table.Cell().PaddingVertical(5).Text(label).SemiBold().FontColor(Muted);
        table.Cell().PaddingVertical(5).Text(value).FontColor(Ink);
    }

    private static string CreateSafeFileName(string value)
    {
        var safeValue = Regex.Replace(value.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(safeValue)
            ? "product"
            : safeValue;
    }

    private static string? LoadDocumentLogoSvg()
    {
        var path = Path.Combine(AppContext.BaseDirectory, DocumentLogoPath);

        return File.Exists(path)
            ? File.ReadAllText(path)
            : null;
    }
}
