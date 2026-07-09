using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private const string DefaultRootPath = "documents";
    private readonly string _rootPath;

    public FileStorageService(IConfiguration configuration)
    {
        var configuredRootPath = configuration["DocumentStorage:LocalRootPath"];

        _rootPath = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            string.IsNullOrWhiteSpace(configuredRootPath)
                ? DefaultRootPath
                : configuredRootPath));

        Directory.CreateDirectory(_rootPath);
    }

    public async Task SaveFileAsync(string key, byte[] content, CancellationToken ct)
    {
        var path = ResolvePath(key);
        var folder = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(folder))
        {
            Directory.CreateDirectory(folder);
        }

        await File.WriteAllBytesAsync(path, content, ct);
    }

    public bool FileExists(string key)
    {
        var path = ResolvePath(key);

        return File.Exists(path);
    }

    public async Task<byte[]> GetFileAsync(string key, CancellationToken ct)
    {
        var path = ResolvePath(key);

        return await File.ReadAllBytesAsync(path, ct);
    }

    private string ResolvePath(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("File storage key is required.");
        }

        var relativeKey = key
            .Trim()
            .TrimStart('/', '\\')
            .Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        var legacyDocumentsPrefix = $"documents{Path.DirectorySeparatorChar}";

        if (relativeKey.StartsWith(legacyDocumentsPrefix, StringComparison.OrdinalIgnoreCase))
        {
            relativeKey = relativeKey[legacyDocumentsPrefix.Length..];
        }

        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativeKey));

        if (!fullPath.StartsWith(_rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(fullPath, _rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("File storage key is outside the configured storage root.");
        }

        return fullPath;
    }
}
