using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Application.Interfaces.Files;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Infrastructure.Files;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    private readonly FileStorageSettings _settings;

    public FileService(IFileRepository fileRepository, IOptions<FileStorageSettings> options)
    {
        _fileRepository = fileRepository;
        _settings = options.Value;
    }

    public async Task<MediaFileDto> UploadImageAsync(Stream fileStream, string fileName, Guid? userId = null)
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

        var savedUrl = $"/uploads/{uniqueFileName}";

        var mediaFile = new MediaFile
        {
            Id = Guid.NewGuid(),
            OriginalFileName = fileName,
            SavedUrl = savedUrl,
            UploadedById = userId
        };

        await _fileRepository.AddAsync(mediaFile);

        return new MediaFileDto { Id = mediaFile.Id, Url = mediaFile.SavedUrl };
    }

    public async Task DeleteFileAsync(Guid fileId)
    {
        var file = await _fileRepository.GetByIdAsync(fileId);
        if (file == null)
        {
            return;
        }

        var fileName = Path.GetFileName(file.SavedUrl);
        var physicalPath = Path.Combine(ResolveUploadPath(), fileName);

        if (File.Exists(physicalPath))
        {
            try
            {
                File.Delete(physicalPath);
            }
            catch (IOException)
            {
            }
        }

        await _fileRepository.DeleteAsync(file);
    }

    private string ResolveUploadPath()
    {
        var configuredPath = _settings.UploadsPath;

        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
    }
}