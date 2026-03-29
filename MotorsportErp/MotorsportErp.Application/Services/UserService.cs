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
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        return UserMapper.ToResponse(user);
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        return UserMapper.ToProfile(user);
    }

    public async Task AssignRoleAsync(Guid adminId, Guid targetUserId, UserRole role)
    {
        var admin = await _userRepository.GetByIdAsync(adminId)
            ?? throw new KeyNotFoundException("Admin not found");

        if (!admin.Roles.HasFlag(UserRole.SuperAdmin))
        {
            throw new UnauthorizedAccessException("Only super admin can assign roles");
        }

        if (role == UserRole.SuperAdmin)
        {
            throw new InvalidOperationException("Cannot assign SuperAdmin role");
        }

        var user = await _userRepository.GetByIdAsync(targetUserId)
            ?? throw new KeyNotFoundException("User not found");

        if (user.Roles.HasFlag(role))
        {
            throw new InvalidOperationException("User already has this role");
        }

        user.Roles |= role;

        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UserUpdateRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        if (string.IsNullOrWhiteSpace(request.Nickname))
            throw new ArgumentException("Nickname cannot be empty");

        user.Nickname = request.Nickname;
        user.Bio = request.Bio;

        await _userRepository.UpdateAsync(user);
    }

    public async Task BlockUserAsync(Guid adminId, Guid targetUserId, bool block)
    {
        var admin = await _userRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");
        if (!admin.Roles.HasFlag(UserRole.Moderator) && !admin.Roles.HasFlag(UserRole.SuperAdmin))
            throw new UnauthorizedAccessException("No permission to block users");

        var targetUser = await _userRepository.GetByIdAsync(targetUserId) ?? throw new KeyNotFoundException("User not found");

        targetUser.IsBlocked = block;
        await _userRepository.UpdateAsync(targetUser);
    }
}