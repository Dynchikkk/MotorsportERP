using MotorsportErp.Application.DTO.Auth;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Security;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Users;

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

        string token = _jwtProvider.GenerateToken(user);
        return AuthMapper.ToResponse(token);
    }
}