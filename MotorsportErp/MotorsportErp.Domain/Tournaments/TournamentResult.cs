using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class TournamentResult : GuidEntity
{
    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
}