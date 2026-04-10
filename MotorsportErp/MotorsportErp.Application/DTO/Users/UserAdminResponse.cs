using MotorsportErp.Application.DTO.Files;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserAdminResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Roles { get; set; }
    public bool IsBlocked { get; set; }
    public int RaceCount { get; set; }
    public MediaFileDto? Avatar { get; set; }
}
