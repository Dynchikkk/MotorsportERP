using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserTournamentEntryResponse
{
    public Guid ApplicationId { get; set; }
    public Guid TournamentId { get; set; }
    public string TournamentName { get; set; } = default!;
    public TournamentStatus TournamentStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TrackId { get; set; }
    public string TrackName { get; set; } = default!;

    public Guid CarId { get; set; }
    public string CarName { get; set; } = default!;

    public TournamentApplicationStatus ApplicationStatus { get; set; }
    public int? Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
}
