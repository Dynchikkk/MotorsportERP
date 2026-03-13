using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Cars;
using MotorsportErp.Models.Entities.Users;

namespace MotorsportErp.Models.Entities.Tournaments;

public class TournamentApplication : GuidEntity
{
    public Guid UserId { get; set; }
    public required User User { get; set; }

    public Guid TournamentId { get; set; }
    public required Tournament Tournament { get; set; }

    public Guid CarId { get; set; }
    public required Car Car { get; set; }

    public bool IsApproved { get; set; }
}