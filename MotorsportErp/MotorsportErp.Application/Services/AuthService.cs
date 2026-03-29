using Microsoft.IdentityModel.Tokens;
using MotorsportErp.Application.DTO.Auth;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Security;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Users;
using System.Security.Claims;

namespace MotorsportErp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> RegisterAsync(UserRegisterRequest request)
    {
        bool exists = await _userRepository.ExistsByEmailAsync(request.Email);
        if (exists)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        string passwordHash = _passwordHasher.Hash(request.Password);
        User user = UserMapper.ToEntity(request, passwordHash);
        await _userRepository.AddAsync(user);

        string token = _jwtProvider.GenerateToken(user);
        return AuthMapper.ToResponse(token);
    }

    public async Task<AuthResponse> LoginAsync(UserLoginRequest request)
    {
        User user = await _userRepository.GetByEmailAsync(request.Email) ?? throw new UnauthorizedAccessException("Invalid email or password");

        bool isValid = _passwordHasher.Verify(request.Password, user.PasswordHash);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (user.IsBlocked)
        {
            throw new UnauthorizedAccessException("Account is blocked");
        }

        var accessToken = _jwtProvider.GenerateToken(user);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _jwtProvider.GetPrincipalFromExpiredToken(accessToken) ?? throw new SecurityTokenException("Invalid access/refresh token");

        var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new SecurityTokenException("Invalid token claims");

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new SecurityTokenException("Invalid request");

        if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new SecurityTokenException("Invalid or expired refresh token");

        var newAccessToken = _jwtProvider.GenerateToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userRepository.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}