using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Interfaces.Files;

namespace MotorsportErp.Infrastructure.Files;

public class LocalMediaFileStorageProvider : IMediaFileStorageProvider
{
    private readonly MediaFileStorageSettings _settings;

    public LocalMediaFileStorageProvider(IOptions<MediaFileStorageSettings> options)
    {
        _settings = options.Value;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType, long fileSize)
    {
        var basePath = MediaFileStoragePathResolver.ResolvePhysicalUploadsPath(_settings.UploadsPath);

        if (fileStream == null || fileSize <= 0)
        {
            throw new ArgumentException("File stream is empty");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required");
        }

        if (fileSize > _settings.MaxUploadSizeBytes)
        {
            throw new ArgumentException("File size exceeds the allowed limit");
        }

        if (string.IsNullOrWhiteSpace(contentType) ||
            !_settings.PhotoAllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid file content type");
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(ext) ||
            !_settings.PhotoAllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Invalid file extension");
        }

        _ = Directory.CreateDirectory(basePath);

        var uniqueFileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(basePath, uniqueFileName);

        await using var fileStreamOnDisk = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            options: FileOptions.Asynchronous);

        await fileStream.CopyToAsync(fileStreamOnDisk);

        return MediaFileStoragePathResolver.BuildPublicFileUrl(_settings.UploadsRequestPath, uniqueFileName);
    }

    public void DeleteFile(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var physicalPath = Path.Combine(
            MediaFileStoragePathResolver.ResolvePhysicalUploadsPath(_settings.UploadsPath),
            fileName);

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }
}
