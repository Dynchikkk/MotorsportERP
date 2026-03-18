using MotorsportErp.Application.DTO.Users;

namespace MotorsportErp.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(Guid id);

    Task<UserProfileResponse> GetProfileAsync(Guid id);
}