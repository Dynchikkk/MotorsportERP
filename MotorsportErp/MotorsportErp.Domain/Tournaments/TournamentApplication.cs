using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class TournamentApplication : GuidEntity
{
    public TournamentApplicationStatus Status { get; set; } = TournamentApplicationStatus.Pending;

    public Guid UserId { get; set; }
    public required User User { get; set; }

    public Guid TournamentId { get; set; }
    public required Tournament Tournament { get; set; }

    public Guid CarId { get; set; }
    public required Car Car { get; set; }
}