namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentApplyRequest
{
    public Guid TournamentId { get; set; }
    public Guid CarId { get; set; }
}