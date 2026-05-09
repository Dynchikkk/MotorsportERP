using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MotorsportErp.Infrastructure.Files;

namespace MotorsportErp.WebApi.Extensions;

public static class MediaFileStorageExtensions
{
    public static IApplicationBuilder UseFileStorage(this IApplicationBuilder app)
    {
        var fileSettings = app.ApplicationServices
            .GetRequiredService<IOptions<MediaFileStorageSettings>>()
            .Value;

        var uploadPhysicalPath = MediaFileStoragePathResolver.ResolvePhysicalUploadsPath(fileSettings.UploadsPath);
        var requestPath = MediaFileStoragePathResolver.NormalizeRequestPath(fileSettings.UploadsRequestPath);

        _ = Directory.CreateDirectory(uploadPhysicalPath);

        _ = app.UseStaticFiles();
        _ = app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadPhysicalPath),
            RequestPath = requestPath,
        });

        return app;
    }
}
