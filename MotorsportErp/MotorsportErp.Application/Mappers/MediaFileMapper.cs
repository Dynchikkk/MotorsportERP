using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Application.Mappers;

public static class MediaFileMapper
{
    public static MediaFileDto ToResponse(MediaFile file)
    {
        return new MediaFileDto
        {
            Id = file.Id,
            Url = file.SavedUrl
        };
    }

    public static List<MediaFileDto> ToResponseList(IEnumerable<MediaFile> files)
    {
        return files.Select(ToResponse).ToList();
    }
}