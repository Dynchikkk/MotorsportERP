using MotorsportErp.Application.Features.MediaFiles.Contracts;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserAdminResponse
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Roles { get; set; }
    public bool IsBlocked { get; set; }
    public int RaceCount { get; set; }
    public MediaFileResponse? Avatar { get; set; }
}
