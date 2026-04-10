using MotorsportErp.Application.DTO.Common;
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Mappers;
using MotorsportErp.Domain.Users;
using System.Linq.Expressions;

namespace MotorsportErp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IFileRepository _fileRepository;

    public UserService(IUserRepository userRepository, IFileRepository fileRepository)
    {
        _userRepository = userRepository;
        _fileRepository = fileRepository;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return UserMapper.ToResponse(user);
    }

    public async Task<UserProfileResponse> GetProfileAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found");
        return UserMapper.ToProfile(user);
    }

    public async Task<PagedResponse<UserResponse>> GetAllAsync(string? search, int page, int pageSize)
    {
        Expression<Func<User, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(search))
        {
            filter = u => u.Nickname.Contains(search);
        }

        var (users, totalCount) = await _userRepository.GetPagedAsync(filter, page, pageSize);

        return new PagedResponse<UserResponse>
        {
            Items = users.Select(UserMapper.ToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResponse<UserAdminResponse>> GetAllAdminAsync(int page, int pageSize)
    {
        var (users, totalCount) = await _userRepository.GetPagedAsync(null, page, pageSize);

        return new PagedResponse<UserAdminResponse>
        {
            Items = users.Select(UserMapper.ToAdminResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
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

        var user = await _userRepository.GetByIdAsync(targetUserId) ?? throw new KeyNotFoundException("User not found");
        if (user.Roles.HasFlag(role))
        {
            throw new InvalidOperationException("User already has this role");
        }

        user.Roles |= role;

        await _userRepository.UpdateAsync(user);
    }

    public async Task UpdateProfileAsync(Guid userId, UserUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nickname))
        {
            throw new ArgumentException("Nickname cannot be empty");
        }

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

        user.Nickname = request.Nickname;
        user.Bio = request.Bio;
        user.Avatar = request.Avatar != null ?
            await _fileRepository.GetByIdAsync(request.Avatar.Id) :
            null;

        await _userRepository.UpdateAsync(user);
    }

    public async Task BlockUserAsync(Guid adminId, Guid targetUserId, bool block)
    {
        var admin = await _userRepository.GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");
        if (!admin.Roles.HasFlag(UserRole.Moderator) && !admin.Roles.HasFlag(UserRole.SuperAdmin))
        {
            throw new UnauthorizedAccessException("No permission to block users");
        }

        var targetUser = await _userRepository.GetByIdAsync(targetUserId) ?? throw new KeyNotFoundException("User not found");

        targetUser.IsBlocked = block;
        await _userRepository.UpdateAsync(targetUser);
    }
}