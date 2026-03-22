using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        User user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return UserMapper.ToResponse(user);
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid id)
    {
        User user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return UserMapper.ToProfile(user);
    }
}