using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Features.Tracks.Contracts;

public class TrackResponse
{
    public Guid Id { get; set; }

    public TrackStatus Status { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public List<MediaFileResponse> Photos { get; set; } = [];

    public int VoteCount { get; set; }
}
