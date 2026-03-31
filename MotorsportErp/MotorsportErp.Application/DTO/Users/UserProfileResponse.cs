using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserProfileResponse : UserResponse
{
    public required string Email { get; set; }
    public UserRole Roles { get; set; }
    public bool IsBlocked { get; set; }

    public int CarsCount { get; set; }
    public int TournamentsCount { get; set; }
}