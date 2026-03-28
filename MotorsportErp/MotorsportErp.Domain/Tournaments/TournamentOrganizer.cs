using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class TournamentOrganizer : GuidEntity
{
    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}