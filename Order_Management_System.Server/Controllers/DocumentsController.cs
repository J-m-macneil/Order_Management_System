using Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize(Roles = "Operations,Admin,Sales")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public DocumentsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetDocumentsForOrder(int orderId)
    {
        var orderExists = await _dbContext.Orders
            .AnyAsync(o => o.OrderId == orderId && o.DeletedAt == null);

        if (!orderExists)
            return NotFound();

        var documents = await _dbContext.Documents
            .Where(d => d.OrderId == orderId)
            .OrderByDescending(d => d.CreatedAt)
            .Select(d => new
            {
                d.DocumentId,
                d.OrderId,
                d.DocumentType,
                d.FileName,
                d.FilePath,
                d.CreatedAt,
                d.CreatedByUserId
            })
            .ToListAsync();

        return Ok(documents);
    }

    [HttpGet("{documentId:int}/download")]
    public async Task<IActionResult> DownloadDocument(int documentId)
    {
        var document = await _dbContext.Documents
            .FirstOrDefaultAsync(d => d.DocumentId == documentId);

        if (document == null)
            return NotFound();

        var documentsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "documents");

        var physicalPath = Path.Combine(documentsFolder, document.FileName);

        if (!System.IO.File.Exists(physicalPath))
            return NotFound("Document file was not found on disk.");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath);

        return File(
            fileBytes,
            "application/pdf",
            document.FileName);
    }
}