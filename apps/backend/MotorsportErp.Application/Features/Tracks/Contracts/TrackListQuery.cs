using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.Features.Tracks.Contracts;

public class TrackListQuery
{
    public string? Search { get; set; }
    public TrackStatus? Status { get; set; }
}
