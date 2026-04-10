using MotorsportErp.Application.DTO.Tournaments;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserProfileResponse : PublicUserProfileResponse
{
    public required string Email { get; set; }
    public UserRole Roles { get; set; }
    public bool IsBlocked { get; set; }
    public List<UserTournamentEntryResponse> Applications { get; set; } = [];
    public List<TournamentResponse> OrganizedTournaments { get; set; } = [];
}
