using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Cars;

public class Car : GuidEntity
{
    public CarClass CarClass { get; set; }

    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public string? Description { get; set; }

    public ICollection<MediaFile> Photos { get; set; } = [];

    // Navigation

    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;

    public ICollection<TournamentApplication> Applications { get; set; } = [];
}