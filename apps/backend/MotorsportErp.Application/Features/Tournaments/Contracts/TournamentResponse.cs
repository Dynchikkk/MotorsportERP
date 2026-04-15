using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public TournamentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TrackId { get; set; }
    public string TrackName { get; set; } = default!;
    public CarClass AllowedCarClass { get; set; }

    public int ParticipantsCount { get; set; }
    public int ApplicationsCount { get; set; }

    public List<MediaFileResponce> Photos { get; set; } = [];
}
