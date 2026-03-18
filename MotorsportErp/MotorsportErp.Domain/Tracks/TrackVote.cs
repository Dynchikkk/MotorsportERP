using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tracks;

public class TrackVote : GuidEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid TrackId { get; set; }
    public Track Track { get; set; } = null!;
}