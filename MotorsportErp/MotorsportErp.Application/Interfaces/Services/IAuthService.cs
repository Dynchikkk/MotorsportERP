using MotorsportErp.Application.DTO.Auth;
using MotorsportErp.Application.DTO.Users;

namespace MotorsportErp.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(UserRegisterRequest request);
    Task<AuthResponse> LoginAsync(UserLoginRequest request);

    Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken);
}