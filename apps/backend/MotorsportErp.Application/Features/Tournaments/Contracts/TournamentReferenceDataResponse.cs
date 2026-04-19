using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentReferenceDataResponse
{
    public int MinRacesToBecomeOrganizer { get; set; }

    public List<EnumValueResponse> TournamentStatuses { get; set; } = [];
    public List<EnumValueResponse> TournamentApplicationStatuses { get; set; } = [];
}
