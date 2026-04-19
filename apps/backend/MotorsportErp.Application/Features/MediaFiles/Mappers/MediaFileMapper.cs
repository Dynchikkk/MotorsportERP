using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Application.Features.MediaFiles.Mappers;

public static class MediaFileMapper
{
    public static MediaFileResponse ToResponse(MediaFile file)
    {
        return new MediaFileResponse
        {
            Id = file.Id,
            Url = file.SavedUrl
        };
    }

    public static List<MediaFileResponse> ToResponseList(IEnumerable<MediaFile> files)
    {
        return files.Select(ToResponse).ToList();
    }
}