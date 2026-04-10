using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentListQuery
{
    public string? Search { get; set; }
    public TournamentStatus? Status { get; set; }
    public Guid? TrackId { get; set; }
}
