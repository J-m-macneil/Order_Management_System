using Application.Features.Products.Commands.CreateSafetyDataSheet;
using Application.Features.Products.Commands.DeleteSafetyDataSheet;
using Application.Features.Products.Commands.GenerateSafetyDataSheet;
using Application.Features.Products.Commands.UpdateSafetyDataSheet;
using Application.Features.Products.DTOs;
using Application.Features.Products.Queries.GetSafetyDataSheetFile;
using Application.Features.Products.Queries.GetSafetyDataSheets;
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

    public ProductSafetyDataSheetsController(IMediator mediator)
    {
        _mediator = mediator;
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
        var result = await _mediator.Send(new GenerateSafetyDataSheetCommand
        {
            ProductId = productId
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{sdsId}/view")]
    public async Task<IActionResult> View(int productId, int sdsId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSafetyDataSheetFileQuery
        {
            ProductId = productId,
            SafetyDataSheetId = sdsId
        }, cancellationToken);

        Response.Headers.ContentDisposition = $"inline; filename=\"{result.FileName}\"";

        return File(result.Content, result.ContentType);
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
