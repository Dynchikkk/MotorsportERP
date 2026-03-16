using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class TournamentOrganizer : GuidEntity
{
    public Guid TournamentId { get; set; }
    public required Tournament Tournament { get; set; }

    public Guid UserId { get; set; }
    public required User User { get; set; }
}