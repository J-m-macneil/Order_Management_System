namespace Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<byte[]> GetFileAsync(string fileName, CancellationToken ct);
    bool FileExists(string fileName);
}