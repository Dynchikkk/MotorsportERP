namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentResultCreateRequest
{
    public Guid UserId { get; set; }

    public int Position { get; set; }
    public TimeSpan? BestLapTime { get; set; }
}