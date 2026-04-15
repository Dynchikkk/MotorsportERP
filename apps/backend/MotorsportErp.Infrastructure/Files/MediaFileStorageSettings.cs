namespace MotorsportErp.Infrastructure.Files;

public class MediaFileStorageSettings
{
    public string UploadsPath { get; set; } = null!;
    public string[] PhotoAllowedExtensions { get; set; } = null!;
}