using MotorsportErp.Models.Base;
using MotorsportErp.Models.Entities.Cars;
using MotorsportErp.Models.Entities.Tournaments;
using MotorsportErp.Models.Entities.Tracks;

namespace MotorsportErp.Models.Entities.Users;

public class User : GuidEntity
{
    public UserRole Role { get; set; } = UserRole.Racer;

    public required string Nickname { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public int RaceCount { get; set; }

    // Navigation

    public ICollection<Car> Cars { get; set; } = [];
    public ICollection<TournamentApplication> Applications { get; set; } = [];
    public ICollection<TournamentResult> Results { get; set; } = [];
    public ICollection<TrackVote> TrackVotes { get; set; } = [];
    public ICollection<Track> CreatedTracks { get; set; } = [];
    public ICollection<Tournament> CreatedTournaments { get; set; } = [];
    public ICollection<TournamentOrganizer> OrganizedTournaments { get; set; } = [];
}