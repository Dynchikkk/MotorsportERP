using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserResponse
{
    public Guid Id { get; set; }

    public UserRole Roles { get; set; }
    public string Nickname { get; set; } = default!;

    public int RaceCount { get; set; }
}