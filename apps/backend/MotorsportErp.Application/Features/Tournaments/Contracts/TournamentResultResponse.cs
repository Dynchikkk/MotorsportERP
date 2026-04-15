using MotorsportErp.Application.Features.Users.Contracts;

namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentResultResponse
{
    public Guid Id { get; set; }
    public int Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
    public UserResponse User { get; set; } = default!;
}
