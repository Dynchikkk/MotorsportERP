namespace MotorsportErp.Application.Features.Tracks.Contracts;

public class TrackUpdateRequest
{
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
}