using Application.Interfaces;
using Domain.Entities.Documents;

namespace Infrastructure.Services.ProcessingJobs;

public class GenerateSdsBundleJobHandler : IProcessingJobHandler
{
    private readonly IOrderDocumentService _documentService;

    public GenerateSdsBundleJobHandler(IOrderDocumentService documentService)
    {
        _documentService = documentService;
    }

    public string JobType => ProcessingJobType.GenerateSdsBundle;

    public async Task HandleAsync(ProcessingJob job, CancellationToken cancellationToken)
    {
        await _documentService.GenerateAsync(
            job.OrderId,
            DocumentType.SafetyDataSheetBundle,
            cancellationToken);
    }
}
