namespace Infrastructure.Services.Documents;

internal static class PdfMergeService
{
    public static byte[] Merge(byte[] mainPdfBytes, IEnumerable<byte[]> pdfsToAppend)
    {
        using var outputDocument = new PdfSharpCore.Pdf.PdfDocument();

        using (var mainStream = new MemoryStream(mainPdfBytes))
        using (var mainDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(
            mainStream,
            PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import))
        {
            foreach (var page in mainDocument.Pages)
            {
                outputDocument.AddPage(page);
            }
        }

        foreach (var pdfBytes in pdfsToAppend)
        {
            using var sdsStream = new MemoryStream(pdfBytes);
            using var sdsDocument = PdfSharpCore.Pdf.IO.PdfReader.Open(
                sdsStream,
                PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.Import);

            foreach (var page in sdsDocument.Pages)
            {
                outputDocument.AddPage(page);
            }
        }

        using var outputStream = new MemoryStream();
        outputDocument.Save(outputStream);
        return outputStream.ToArray();
    }
}
