using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tracks;

public class Track : GuidEntity
{
    public TrackStatus Status { get; set; } = TrackStatus.Unofficial;

    public required string Name { get; set; }
    public required string Location { get; set; }

    public int ConfirmationThreshold { get; set; } = 10;
    public int VoteCount => Votes.Count(v => v.IsPositive);

    public ICollection<MediaFile> Photos { get; set; } = [];

    // Navigation

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public ICollection<TrackVote> Votes { get; set; } = [];
    public ICollection<Tournament> Tournaments { get; set; } = [];
}