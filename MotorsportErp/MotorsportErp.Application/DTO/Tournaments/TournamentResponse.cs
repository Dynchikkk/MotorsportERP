using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public TournamentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TrackId { get; set; }

    public int ParticipantsCount { get; set; }
}