using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class TournamentApplication : GuidEntity
{
    public TournamentApplicationStatus Status { get; set; } = TournamentApplicationStatus.Pending;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    public Guid CarId { get; set; }
    public Car Car { get; set; } = null!;
}