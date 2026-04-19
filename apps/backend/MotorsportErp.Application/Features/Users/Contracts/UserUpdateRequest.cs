using MotorsportErp.Application.Features.MediaFiles.Contracts;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserUpdateRequest
{
    public required string Nickname { get; set; }
    public string? Bio { get; set; }
    public MediaFileResponse? Avatar { get; set; }
}