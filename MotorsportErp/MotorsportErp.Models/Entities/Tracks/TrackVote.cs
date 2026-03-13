using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Users;

namespace MotorsportErp.Models.Entities.Tracks;

public class TrackVote : GuidEntity
{
    public Guid UserId { get; set; }
    public required User User { get; set; }

    public Guid TrackId { get; set; }
    public required Track Track { get; set; }
}