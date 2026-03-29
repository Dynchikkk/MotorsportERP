namespace MotorsportErp.Application.DTO.Users;

public class UserUpdateRequest
{
    public string Nickname { get; set; } = default!;
    public string? Bio { get; set; }
}