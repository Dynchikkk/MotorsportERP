namespace MotorsportErp.Application.DTO.Auth;

public class UserLoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}