using Application.Common.Interfaces;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public FileStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "documents");
    }

    public bool FileExists(string fileName)
    {
        var path = Path.Combine(_basePath, fileName);
        return File.Exists(path);
    }

    public async Task<byte[]> GetFileAsync(string fileName, CancellationToken ct)
    {
        var path = Path.Combine(_basePath, fileName);
        return await File.ReadAllBytesAsync(path, ct);
    }
}