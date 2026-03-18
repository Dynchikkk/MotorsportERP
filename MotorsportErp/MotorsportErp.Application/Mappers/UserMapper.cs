
using MotorsportErp.Application.DTO.Users;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Application.Mappers;
public static class UserMapper
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Roles = user.Roles,
            RaceCount = user.RaceCount
        };
    }

    public static UserProfileResponse ToProfile(User user)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Roles = user.Roles,
            RaceCount = user.RaceCount,
            CarsCount = user.Cars.Count,
            TournamentsCount = user.Applications.Count
        };
    }

    public static User ToEntity(UserRegisterRequest request, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Nickname = request.Nickname,
            PasswordHash = passwordHash
        };
    }
}