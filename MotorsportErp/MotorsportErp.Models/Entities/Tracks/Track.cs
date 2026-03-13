using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Tournaments;
using MotorsportErp.Models.Entities.Users;

namespace MotorsportErp.Models.Entities.Tracks;

public class Track : GuidEntity
{
    public TrackStatus Status { get; set; } = TrackStatus.Unofficial;

    public required string Name { get; set; }
    public required string Location { get; set; }
    public string? LayoutImageUrl { get; set; }

    public int PopularityVotes { get; set; }
    public int ConfirmationThreshold { get; set; } = 10;

    // Navigation

    public Guid CreatedById { get; set; }
    public required User CreatedBy { get; set; }

    public ICollection<TrackVote> Votes { get; set; } = [];
    public ICollection<Tournament> Tournaments { get; set; } = [];
}