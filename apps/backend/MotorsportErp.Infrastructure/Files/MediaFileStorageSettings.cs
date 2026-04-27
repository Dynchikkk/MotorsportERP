namespace MotorsportErp.Infrastructure.Files;

public class MediaFileStorageSettings
{
    public string UploadsPath { get; set; } = null!;
    public string UploadsRequestPath { get; set; } = "/uploads";
    public long MaxUploadSizeBytes { get; set; }
    public string[] PhotoAllowedExtensions { get; set; } = [];
    public string[] PhotoAllowedContentTypes { get; set; } = [];
}
