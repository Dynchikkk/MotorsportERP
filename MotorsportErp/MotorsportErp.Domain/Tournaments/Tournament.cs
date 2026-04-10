using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Domain.Tournaments;

public class Tournament : GuidEntity
{
    public TournamentStatus Status { get; set; } = TournamentStatus.RegistrationOpen;

    public required string Name { get; set; }
    public required string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public CarClass AllowedCarClass { get; set; }
    public int RequiredRaceCount { get; set; }
    public int RequiredParticipants { get; set; }

    public ICollection<MediaFile> Photos { get; set; } = [];

    // Navigation

    public Guid TrackId { get; set; }
    public Track Track { get; set; } = null!;

    public Guid CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<TournamentApplication> Applications { get; set; } = [];
    public ICollection<TournamentResult> Results { get; set; } = [];
    public ICollection<TournamentOrganizer> Organizers { get; set; } = [];
}