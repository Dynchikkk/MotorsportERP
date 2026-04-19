using MotorsportErp.Application.Features.MediaFiles.Contracts;

namespace MotorsportErp.Application.Features.Users.Contracts;

public class UserResponse
{
    public Guid Id { get; set; }
    public required string Nickname { get; set; }
    public string? Bio { get; set; }
    public int RaceCount { get; set; }
    public MediaFileResponse? Avatar { get; set; }
}