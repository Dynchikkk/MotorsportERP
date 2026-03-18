using MotorsportErp.Application.DTO.Auth;

namespace MotorsportErp.Application.Mappers;

public static class AuthMapper
{
    public static AuthResponse ToResponse(string token)
    {
        return new AuthResponse
        {
            AccessToken = token
        };
    }
}