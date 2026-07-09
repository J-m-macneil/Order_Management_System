using Application.Interfaces;

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
        await _documentService.GenerateForJobAsync(job, cancellationToken);
    }
}
