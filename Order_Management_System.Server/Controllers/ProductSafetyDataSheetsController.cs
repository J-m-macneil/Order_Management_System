using Application.Common.Interfaces;
using Application.Features.Products.Commands.CreateSafetyDataSheet;
using Application.Features.Products.Commands.DeleteSafetyDataSheet;
using Application.Features.Products.Commands.UpdateSafetyDataSheet;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries.GetSafetyDataSheets;
using Application.Interfaces;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;

[ApiController]
[Route("api/products/{productId}/sds")]
[Authorize]
public class ProductSafetyDataSheetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISafetyDataSheetRepository _sdsRepository;
    private readonly ISafetyDataSheetDocumentGenerator _sdsDocumentGenerator;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _fileStorage;

    public ProductSafetyDataSheetsController(
        IMediator mediator,
        ISafetyDataSheetRepository sdsRepository,
        ISafetyDataSheetDocumentGenerator sdsDocumentGenerator,
        ICurrentUserService currentUser,
        IFileStorageService fileStorage)
    {
        _mediator = mediator;
        _sdsRepository = sdsRepository;
        _sdsDocumentGenerator = sdsDocumentGenerator;
        _currentUser = currentUser;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SafetyDataSheetDto>>> Get(int productId)
    {
        var result = await _mediator.Send(new GetSafetyDataSheetsQuery
        {
            ProductId = productId
        });

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SafetyDataSheetDto>> Create(
        int productId,
        [FromBody] CreateSafetyDataSheetCommand command)
    {
        command.ProductId = productId;

        var result = await _mediator.Send(command);

        return Ok(result);
    }

    [HttpPost("generate")]
    [Authorize(Policy = "OperationsOrAdmin")]
    public async Task<ActionResult<SafetyDataSheetDto>> Generate(int productId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not int userId)
        {
            return Unauthorized();
        }

        try
        {
            var result = await _sdsDocumentGenerator.GenerateAsync(productId, userId, cancellationToken);

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{sdsId}/view")]
    public async Task<IActionResult> View(int productId, int sdsId, CancellationToken cancellationToken)
    {
        var sds = await _sdsRepository.GetByIdAsync(productId, sdsId, cancellationToken);

        if (sds == null)
        {
            return NotFound();
        }

        if (!_fileStorage.FileExists(sds.FilePath))
        {
            return NotFound("SDS file was not found on disk.");
        }

        var fileBytes = await _fileStorage.GetFileAsync(sds.FilePath, cancellationToken);

        Response.Headers.ContentDisposition = $"inline; filename=\"{sds.FileName}\"";

        return File(fileBytes, "application/pdf");
    }

    [HttpPut("{sdsId}")]
    public async Task<IActionResult> Update(
        int productId,
        int sdsId,
        [FromBody] UpdateSafetyDataSheetCommand command)
    {
        command.ProductId = productId;
        command.SafetyDataSheetId = sdsId;

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{sdsId}")]
    public async Task<IActionResult> Delete(int productId, int sdsId)
    {
        await _mediator.Send(new DeleteSafetyDataSheetCommand
        {
            ProductId = productId,
            SafetyDataSheetId = sdsId
        });

        return NoContent();
    }
}
