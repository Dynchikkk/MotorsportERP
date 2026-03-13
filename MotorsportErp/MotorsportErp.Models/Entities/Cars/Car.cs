using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Tournaments;
using MotorsportErp.Models.Entities.Users;

namespace MotorsportErp.Models.Entities.Cars;

public class Car : GuidEntity
{
    public CarClass CarClass { get; set; }

    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }

    // Navigation

    public Guid OwnerId { get; set; }
    public required User Owner { get; set; }

    public ICollection<TournamentApplication> Applications { get; set; } = [];
}