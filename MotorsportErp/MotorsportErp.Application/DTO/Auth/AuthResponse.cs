namespace MotorsportErp.Application.DTO.Auth;

public class AuthResponse
{
    public required string AccessToken { get; set; } 
    public required string RefreshToken { get; set; }
}