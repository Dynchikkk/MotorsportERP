using MotorsportErp.Application.DTO.Users;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentResultResponse
{
    public Guid Id { get; set; }
    public int Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
    public UserResponse User { get; set; } = default!;
}
