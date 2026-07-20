namespace Application.Common.Interfaces;

public interface IFileStorageService
{
    Task SaveFileAsync(string key, byte[] content, CancellationToken ct);
    Task<byte[]> GetFileAsync(string key, CancellationToken ct);
    bool FileExists(string key);
}
