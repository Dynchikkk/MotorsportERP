using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tracks;

public class TrackVote : GuidEntity
{
    public Guid UserId { get; set; }
    public required User User { get; set; }

    public Guid TrackId { get; set; }
    public required Track Track { get; set; }
}