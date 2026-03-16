using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Application.DTO.Tournaments;

public class TournamentCreateRequest
{
    public required string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid TrackId { get; set; }
    public CarClass AllowedCarClass { get; set; }

    public required string Description { get; set; }

    public int RequiredRaceCount { get; set; }
    public int RequiredParticipants { get; set; }
}