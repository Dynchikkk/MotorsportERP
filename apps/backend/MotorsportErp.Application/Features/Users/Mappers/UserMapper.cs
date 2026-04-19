using MotorsportErp.Application.Features.Auth.Contracts;
using MotorsportErp.Application.Features.MediaFiles.Mappers;
using MotorsportErp.Application.Features.Users.Contracts;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Features.Users.Mappers;

public static class UserMapper
{
    /// <summary>
    /// Maps a User domain entity to a public UserResponse DTO.
    /// Excludes sensitive data like email, roles, and block status.
    /// </summary>
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Bio = user.Bio,
            RaceCount = user.RaceCount,
            Avatar = user.Avatar != null ? MediaFileMapper.ToResponse(user.Avatar) : null
        };
    }

    public static PublicUserProfileResponse ToPublicProfile(User user)
    {
        return new PublicUserProfileResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Bio = user.Bio,
            RaceCount = user.RaceCount,
            Avatar = user.Avatar != null ? MediaFileMapper.ToResponse(user.Avatar) : null
        };
    }

    /// <summary>
    /// Maps a User domain entity to a detailed UserProfileResponse DTO.
    /// Includes private data and aggregate counts for cars and tournaments.
    /// </summary>
    public static UserProfileResponse ToProfile(User user)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Bio = user.Bio,
            RaceCount = user.RaceCount,
            Email = user.Email,
            Roles = user.Roles,
            IsBlocked = user.IsBlocked,
            Avatar = user.Avatar != null ? MediaFileMapper.ToResponse(user.Avatar) : null
        };
    }

    /// <summary>
    /// Maps a registration request to a new User domain entity.
    /// Assigns a new Guid and the default Racer role.
    /// </summary>
    public static User ToEntity(UserRegisterRequest request, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Nickname = request.Nickname,
            PasswordHash = passwordHash,
            Roles = UserRole.Racer,
            IsBlocked = false
        };
    }

    public static UserAdminResponse ToAdminResponse(User user)
    {
        return new UserAdminResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Email = user.Email,
            Roles = user.Roles,
            IsBlocked = user.IsBlocked,
            RaceCount = user.RaceCount,
            Avatar = user.Avatar != null ? MediaFileMapper.ToResponse(user.Avatar) : null
        };
    }
}
