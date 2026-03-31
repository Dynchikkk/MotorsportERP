namespace MotorsportErp.Application.DTO.Users;

public class UserUpdateRequest
{
    public required string Nickname { get; set; }
    public string? Bio { get; set; }
}