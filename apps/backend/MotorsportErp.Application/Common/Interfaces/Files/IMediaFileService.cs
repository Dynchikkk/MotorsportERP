using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Common.Interfaces.Files;

public interface IMediaFileService
{
    Task<MediaFileResponce> UploadImageAsync(Stream fileStream, string fileName, Guid? userId = null);
    Task DeleteFileAsync(Guid fileId, Guid ownerId);
}