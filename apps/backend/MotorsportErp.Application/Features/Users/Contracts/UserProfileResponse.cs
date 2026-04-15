using MotorsportErp.Application.Features.Tournaments.Contracts;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserProfileResponse : PublicUserProfileResponse
{
    public required string Email { get; set; }
    public UserRole Roles { get; set; }
    public bool IsBlocked { get; set; }
    public List<UserTournamentEntryResponse> Applications { get; set; } = [];
    public List<TournamentResponse> OrganizedTournaments { get; set; } = [];
}
