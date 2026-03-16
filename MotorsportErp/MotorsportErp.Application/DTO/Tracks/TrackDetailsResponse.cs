using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Application.DTO.Tracks;

public class TrackDetailsResponse
{
    public Guid Id { get; set; }

    public TrackStatus Status { get; set; }
    public string Name { get; set; } = default!;
    public string Location { get; set; } = default!;
    public string? LayoutImageUrl { get; set; }

    public int VoteCount { get; set; }
    public int ConfirmationThreshold { get; set; }
}