namespace MotorsportErp.Application.Common.Interfaces.Files;

public interface IMediaFileStorageProvider
{
    Task<string> UploadFileAsync(Stream fileStream, string extension);
    void DeleteFile(string fileUrl);
}