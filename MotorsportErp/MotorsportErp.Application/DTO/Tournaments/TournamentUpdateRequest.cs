namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentUpdateRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = default!;

    public int RequiredParticipants { get; set; }
}