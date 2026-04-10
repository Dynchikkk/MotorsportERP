using MotorsportErp.Application.DTO.Files;

namespace MotorsportErp.Application.Interfaces.Files;

public interface IFileService
{
    Task<MediaFileDto> UploadImageAsync(Stream fileStream, string fileName, Guid? userId = null);
    Task DeleteFileAsync(Guid fileId, Guid ownerId);
}