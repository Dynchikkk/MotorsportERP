using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserProfileResponse> GetProfileAsync(Guid id);

    Task AssignRoleAsync(Guid adminId, Guid targetUserId, UserRole role);

    Task UpdateProfileAsync(Guid userId, UserUpdateRequest request);

    Task BlockUserAsync(Guid adminId, Guid targetUserId, bool block);
}