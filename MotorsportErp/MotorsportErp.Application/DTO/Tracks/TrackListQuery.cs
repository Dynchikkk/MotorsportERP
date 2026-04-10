using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.DTO.Tracks;

public class TrackListQuery
{
    public string? Search { get; set; }
    public TrackStatus? Status { get; set; }
}
