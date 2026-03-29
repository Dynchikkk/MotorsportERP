using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Domain.Users;

public class User : GuidEntity
{
    public UserRole Roles { get; set; } = UserRole.Racer;

    public required string Nickname { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Bio { get; set; }

    public int RaceCount { get; set; }
    public bool IsBlocked { get; set; } = false;

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation

    public ICollection<Car> Cars { get; set; } = [];
    public ICollection<TournamentApplication> Applications { get; set; } = [];
    public ICollection<TournamentResult> Results { get; set; } = [];
    public ICollection<TrackVote> TrackVotes { get; set; } = [];
    public ICollection<Track> CreatedTracks { get; set; } = [];
    public ICollection<Tournament> CreatedTournaments { get; set; } = [];
    public ICollection<TournamentOrganizer> OrganizedTournaments { get; set; } = [];
}