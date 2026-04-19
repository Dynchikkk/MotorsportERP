using MotorsportErp.Application.Features.MediaFiles.Contracts;

namespace MotorsportErp.Application.Features.MediaFiles.Interfaces;

public interface IMediaFileService
{
    Task<MediaFileResponse> UploadImageAsync(Stream fileStream, string fileName, Guid userId);
    Task DeleteFileAsync(Guid fileId, Guid ownerId);
}