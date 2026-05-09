using Microsoft.Extensions.Options;

namespace MotorsportErp.Infrastructure.Files;

public class MediaFileStorageSettingsValidator : IValidateOptions<MediaFileStorageSettings>
{
    public ValidateOptionsResult Validate(string? name, MediaFileStorageSettings options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.UploadsPath))
        {
            failures.Add("MediaFileStorageSettings:UploadsPath is required.");
        }

        if (string.IsNullOrWhiteSpace(options.UploadsRequestPath))
        {
            failures.Add("MediaFileStorageSettings:UploadsRequestPath is required.");
        }
        else if (!options.UploadsRequestPath.StartsWith('/'))
        {
            failures.Add("MediaFileStorageSettings:UploadsRequestPath must start with '/'.");
        }

        if (options.MaxUploadSizeBytes <= 0)
        {
            failures.Add("MediaFileStorageSettings:MaxUploadSizeBytes must be greater than zero.");
        }

        if (options.PhotoAllowedExtensions.Length == 0)
        {
            failures.Add("MediaFileStorageSettings:PhotoAllowedExtensions must contain at least one extension.");
        }
        else if (options.PhotoAllowedExtensions.Any(ext => string.IsNullOrWhiteSpace(ext) || !ext.StartsWith('.')))
        {
            failures.Add("MediaFileStorageSettings:PhotoAllowedExtensions must contain extensions in '.ext' format.");
        }

        if (options.PhotoAllowedContentTypes.Length == 0)
        {
            failures.Add("MediaFileStorageSettings:PhotoAllowedContentTypes must contain at least one content type.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
