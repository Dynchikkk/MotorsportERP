using MotorsportErp.Application.DTO.Auth; // Needed for UserRegisterRequest
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Mappers;

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
            RaceCount = user.RaceCount
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

            // Assuming the collections are initialized or eagerly loaded via .Include()
            CarsCount = user.Cars?.Count ?? 0,
            TournamentsCount = user.Applications?.Count ?? 0
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
            Roles = UserRole.Racer, // Default role for all new registrations
            IsBlocked = false
        };
    }
}