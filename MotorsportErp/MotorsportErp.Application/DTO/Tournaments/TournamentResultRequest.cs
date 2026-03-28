namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentResultRequest
{
    public Guid UserId { get; set; }

    public int Position { get; set; }

    public int Points { get; set; }
}