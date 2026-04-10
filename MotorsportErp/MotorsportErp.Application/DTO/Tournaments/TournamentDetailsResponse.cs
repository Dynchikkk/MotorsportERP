using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Application.DTO.Tracks;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentDetailsResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public TournamentStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TrackId { get; set; }
    public string Description { get; set; } = default!;
    public CarClass AllowedCarClass { get; set; }
    public int RequiredRaceCount { get; set; }

    public int ParticipantsCount { get; set; }
    public int ApplicationsCount { get; set; }
    public int RequiredParticipants { get; set; }

    public TrackResponse? Track { get; set; }
    public List<UserResponse> Organizers { get; set; } = [];
    public List<TournamentApplicationResponse> Participants { get; set; } = [];
    public List<TournamentResultResponse> Results { get; set; } = [];
    public List<MediaFileDto> Photos { get; set; } = [];
}
