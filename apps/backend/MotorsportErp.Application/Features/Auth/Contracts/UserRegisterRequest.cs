namespace MotorsportErp.Application.Features.Auth.Contracts;

public class UserRegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    public required string Nickname { get; set; }
}