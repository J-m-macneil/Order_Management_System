using Application.Interfaces;
using Domain.Entities.Documents;

namespace Infrastructure.Services.ProcessingJobs;

public class GenerateOrderSummaryJobHandler : IProcessingJobHandler
{
    private readonly IOrderDocumentService _documentService;

    public GenerateOrderSummaryJobHandler(IOrderDocumentService documentService)
    {
        _documentService = documentService;
    }

    public string JobType => ProcessingJobType.GenerateOrderSummaryDocument;

    public async Task HandleAsync(ProcessingJob job, CancellationToken cancellationToken)
    {
        await _documentService.GenerateAsync(
            job.OrderId,
            DocumentType.OrderSummary,
            cancellationToken);
    }
}
