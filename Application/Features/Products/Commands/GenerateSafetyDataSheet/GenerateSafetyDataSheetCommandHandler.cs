using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Products.DTOs;
using Application.Interfaces;
using MediatR;

namespace Application.Features.Products.Commands.GenerateSafetyDataSheet;

public class GenerateSafetyDataSheetCommandHandler
    : IRequestHandler<GenerateSafetyDataSheetCommand, SafetyDataSheetDto>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ISafetyDataSheetDocumentGenerator _documentGenerator;

    public GenerateSafetyDataSheetCommandHandler(
        ICurrentUserService currentUser,
        ISafetyDataSheetDocumentGenerator documentGenerator)
    {
        _currentUser = currentUser;
        _documentGenerator = documentGenerator;
    }

    public async Task<SafetyDataSheetDto> Handle(GenerateSafetyDataSheetCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not int userId)
        {
            throw new UnauthorizedException("You must be signed in to generate an SDS.");
        }

        return await _documentGenerator.GenerateAsync(request.ProductId, userId, ct);
    }
}
