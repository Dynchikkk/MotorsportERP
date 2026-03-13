using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Users;

namespace MotorsportErp.Models.Entities.Tournaments;

public class TournamentResult : GuidEntity
{
    public Guid TournamentId { get; set; }
    public required Tournament Tournament { get; set; }

    public Guid UserId { get; set; }
    public required User User { get; set; }

    public int Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
}