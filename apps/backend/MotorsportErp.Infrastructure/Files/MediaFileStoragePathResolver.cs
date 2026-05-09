namespace MotorsportErp.Infrastructure.Files;

public static class MediaFileStoragePathResolver
{
    public static string ResolvePhysicalUploadsPath(string configuredPath)
    {
        return Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(Directory.GetCurrentDirectory(), configuredPath);
    }

    public static string NormalizeRequestPath(string configuredRequestPath)
    {
        return "/" + configuredRequestPath.Trim().Trim('/');
    }

    public static string BuildPublicFileUrl(string configuredRequestPath, string fileName)
    {
        return $"{NormalizeRequestPath(configuredRequestPath)}/{fileName}";
    }
}
