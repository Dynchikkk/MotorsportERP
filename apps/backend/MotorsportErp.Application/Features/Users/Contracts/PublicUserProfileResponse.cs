using MotorsportErp.Application.Features.Cars.Contracts;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class PublicUserProfileResponse : UserResponse
{
    public int CarsCount { get; set; }
    public int TournamentsCount { get; set; }

    public List<CarResponse> Cars { get; set; } = [];
    public List<UserTournamentEntryResponse> CurrentTournaments { get; set; } = [];
    public List<UserTournamentEntryResponse> TournamentHistory { get; set; } = [];
}
