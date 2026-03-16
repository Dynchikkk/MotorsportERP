namespace MotorsportErp.Application.DTO.Tracks;

public class TrackCreateRequest
{
    public required string Name { get; set; }
    public required string Location { get; set; }
    public string? LayoutImageUrl { get; set; }
}