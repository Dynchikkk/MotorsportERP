using MotorsportErp.Application.Features.Auth.Contracts;
using MotorsportErp.Application.Features.Users.Contracts;

namespace MotorsportErp.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(UserRegisterRequest request);
    Task<AuthResponse> LoginAsync(UserLoginRequest request);
    Task RevokeTokenAsync(Guid userId);

    Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken);
}