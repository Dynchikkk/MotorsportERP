using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserProfileResponse
{
    public Guid Id { get; set; }

    public UserRole Roles { get; set; }
    public string Nickname { get; set; } = default!;

    public int RaceCount { get; set; }
    public int CarsCount { get; set; }
    public int TournamentsCount { get; set; }
}