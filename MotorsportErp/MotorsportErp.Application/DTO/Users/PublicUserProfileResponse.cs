using MotorsportErp.Application.DTO.Cars;

namespace MotorsportErp.Application.DTO.Users;

public class PublicUserProfileResponse : UserResponse
{
    public int CarsCount { get; set; }
    public int TournamentsCount { get; set; }

    public List<CarResponse> Cars { get; set; } = [];
    public List<UserTournamentEntryResponse> CurrentTournaments { get; set; } = [];
    public List<UserTournamentEntryResponse> TournamentHistory { get; set; } = [];
}
