using MotorsportErp.Application.Common.Contracts;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserUpdateRequest
{
    public required string Nickname { get; set; }
    public string? Bio { get; set; }
    public MediaFileResponce? Avatar { get; set; }
}