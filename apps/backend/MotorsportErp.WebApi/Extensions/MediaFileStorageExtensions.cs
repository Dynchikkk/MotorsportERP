using Microsoft.Extensions.FileProviders;
using MotorsportErp.Infrastructure.Files;

namespace MotorsportErp.WebApi.Extensions;

public static class MediaFileStorageExtensions
{
    public static IApplicationBuilder UseFileStorage(this IApplicationBuilder app, IConfiguration configuration)
    {
        var fileSettings = configuration.GetSection("MediaFileStorageSettings").Get<MediaFileStorageSettings>();
        if (fileSettings == null || string.IsNullOrEmpty(fileSettings.UploadsPath))
        {
            return app;
        }

        var uploadPhysicalPath = Path.IsPathRooted(fileSettings.UploadsPath)
            ? fileSettings.UploadsPath
            : Path.Combine(Directory.GetCurrentDirectory(), fileSettings.UploadsPath);
        if (!Directory.Exists(uploadPhysicalPath))
        {
            _ = Directory.CreateDirectory(uploadPhysicalPath);
        }

        _ = app.UseStaticFiles();
        _ = app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadPhysicalPath),
            RequestPath = "/uploads",
        });

        return app;
    }
}
