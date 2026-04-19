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

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var basePath = ResolveUploadPath();

        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream is empty");
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!_settings.PhotoAllowedExtensions.Contains(ext))
        {
            throw new ArgumentException("Invalid file extension");
        }

        if (!Directory.Exists(basePath))
        {
            _ = Directory.CreateDirectory(basePath);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(basePath, uniqueFileName);

        using (var fileStreamOnDisk = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOnDisk);
        }

        return $"/uploads/{uniqueFileName}";
    }

    public void DeleteFile(string fileUrl)
    {
        var fileName = Path.GetFileName(fileUrl);
        var physicalPath = Path.Combine(ResolveUploadPath(), fileName);
        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }

    private string ResolveUploadPath()
    {
        var configuredPath = _settings.UploadsPath;

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
    }
}
