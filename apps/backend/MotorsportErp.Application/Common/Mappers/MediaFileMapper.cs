using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Application.Common.Mappers;

public static class MediaFileMapper
{
    public static MediaFileResponce ToResponse(MediaFile file)
    {
        return new MediaFileResponce
        {
            Id = file.Id,
            Url = file.SavedUrl
        };
    }

    public static List<MediaFileResponce> ToResponseList(IEnumerable<MediaFile> files)
    {
        return files.Select(ToResponse).ToList();
    }
}