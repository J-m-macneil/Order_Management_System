using Application.Common.Interfaces;
using Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/documents")]
[Authorize(Roles = "Operations,Admin,Sales,Demo")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentRepository _repo;
    private readonly IFileStorageService _fileStorage;

    public DocumentsController(
        IDocumentRepository repo,
        IFileStorageService fileStorage)
    {
        _repo = repo;
        _fileStorage = fileStorage;
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetDocumentsForOrder(int orderId, CancellationToken ct)
    {
        var documents = await _repo.GetByOrderIdAsync(orderId, ct);

        var result = documents.Select(d => new
        {
            d.DocumentId,
            d.OrderId,
            d.DocumentType,
            d.FileName,
            d.FilePath,
            d.CreatedAt,
            d.CreatedByUserId
        });

        return Ok(result);
    }

    [HttpGet("{documentId:int}/download")]
    public async Task<IActionResult> DownloadDocument(int documentId, CancellationToken ct)
    {
        var document = await _repo.GetByIdAsync(documentId, ct);

        if (document == null)
            return NotFound();

        if (!_fileStorage.FileExists(document.FilePath))
            return NotFound("Document file was not found on disk.");

        var fileBytes = await _fileStorage.GetFileAsync(document.FilePath, ct);

        return File(fileBytes, "application/pdf", document.FileName);
    }
}
