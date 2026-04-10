using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.DTO.Tracks;

public class TrackResponse
{
    public Guid Id { get; set; }

    public TrackStatus Status { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public List<MediaFileDto> Photos { get; set; } = [];

    public int VoteCount { get; set; }
}
