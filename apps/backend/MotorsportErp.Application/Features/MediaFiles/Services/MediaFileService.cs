using MotorsportErp.Application.Common.Exceptions;
using MotorsportErp.Application.Common.Interfaces.Files;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Application.Features.MediaFiles.Interfaces;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Application.Features.MediaFiles.Services;

public class MediaFileService : IMediaFileService
{
    private readonly IFileRepository _fileRepository;
    private readonly IMediaFileStorageProvider _storageProvider;

    public MediaFileService(IFileRepository fileRepository, IMediaFileStorageProvider storageProvider)
    {
        _fileRepository = fileRepository;
        _storageProvider = storageProvider;
    }

    public async Task<MediaFileResponse> UploadImageAsync(Stream fileStream, string fileName, Guid userId)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var savedUrl = await _storageProvider.UploadFileAsync(fileStream, ext);

        var mediaFile = new MediaFile
        {
            Id = Guid.NewGuid(),
            OriginalFileName = fileName,
            SavedUrl = savedUrl,
            UploadedById = userId
        };

        await _fileRepository.AddAsync(mediaFile);
        return new MediaFileResponse { Id = mediaFile.Id, Url = mediaFile.SavedUrl };
    }

    public async Task DeleteFileAsync(Guid fileId, Guid ownerId)
    {
        var file = await _fileRepository.GetByIdAsync(fileId) ?? throw new KeyNotFoundException();
        if (file.UploadedById != ownerId)
        {
            throw new ForbiddenException();
        }

        _storageProvider.DeleteFile(file.SavedUrl);
        await _fileRepository.DeleteAsync(file);
    }
}
