using MotorsportErp.Application.Common.Contracts;
using MotorsportErp.Application.Common.Interfaces.Services;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Features.Users.Interfaces;

public interface IUserService : IReferenceDataServices<UserReferenceDataResponce>
{
    Task<UserResponse> GetByIdAsync(Guid id);
    Task<PublicUserProfileResponse> GetPublicProfileAsync(Guid id);
    Task<UserProfileResponse> GetProfileAsync(Guid id);

    Task<PagedResponse<UserResponse>> GetAllAsync(string? search, int page = 0, int pageSize = 20);
    Task<PagedResponse<UserAdminResponse>> GetAllAdminAsync(int page = 0, int pageSize = 20);

    Task UpdateProfileAsync(Guid userId, UserUpdateRequest request);

    Task AssignRoleAsync(Guid adminId, Guid targetUserId, UserRole role);
    Task BlockUserAsync(Guid adminId, Guid targetUserId, bool block);
}
