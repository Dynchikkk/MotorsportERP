using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.DTO.Tracks;

public class TrackCreateRequest
{
    public required string Name { get; set; }
    public required string Location { get; set; }

    public TrackStatus? Status { get; set; }
}