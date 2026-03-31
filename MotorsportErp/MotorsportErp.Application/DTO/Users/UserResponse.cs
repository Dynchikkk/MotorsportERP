using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.DTO.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public required string Nickname { get; set; }
    public string? Bio { get; set; }
    public int RaceCount { get; set; }
}