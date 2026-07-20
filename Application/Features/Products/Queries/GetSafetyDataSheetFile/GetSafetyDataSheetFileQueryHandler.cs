using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Products.Queries.GetSafetyDataSheetFile;

public class GetSafetyDataSheetFileQueryHandler
    : IRequestHandler<GetSafetyDataSheetFileQuery, SafetyDataSheetFileDto>
{
    private readonly ISafetyDataSheetRepository _repo;
    private readonly IFileStorageService _fileStorage;

    public GetSafetyDataSheetFileQueryHandler(
        ISafetyDataSheetRepository repo,
        IFileStorageService fileStorage)
    {
        _repo = repo;
        _fileStorage = fileStorage;
    }

    public async Task<SafetyDataSheetFileDto> Handle(GetSafetyDataSheetFileQuery request, CancellationToken ct)
    {
        var sds = await _repo.GetByIdAsync(request.ProductId, request.SafetyDataSheetId, ct)
            ?? throw new NotFoundException("Safety data sheet", request.SafetyDataSheetId);

        if (!_fileStorage.FileExists(sds.FilePath))
        {
            throw new NotFoundException("SDS file was not found.");
        }

        return new SafetyDataSheetFileDto
        {
            FileName = sds.FileName,
            Content = await _fileStorage.GetFileAsync(sds.FilePath, ct)
        };
    }
}
