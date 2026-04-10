namespace MotorsportErp.Infrastructure.Files;

public class FileStorageSettings
{
    public string UploadsPath { get; set; } = null!;
    public string[] PhotoAllowedExtensions { get; set; } = null!;
}