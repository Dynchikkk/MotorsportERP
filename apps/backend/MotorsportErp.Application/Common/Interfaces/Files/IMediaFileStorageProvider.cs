namespace MotorsportErp.Application.Common.Interfaces.Files;

public interface IMediaFileStorageProvider
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string? contentType, long fileSize);
    void DeleteFile(string fileUrl);
}
