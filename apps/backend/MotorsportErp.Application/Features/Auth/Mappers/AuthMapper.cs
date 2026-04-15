using MotorsportErp.Application.Features.Auth.Contracts;

namespace MotorsportErp.Application.Features.Auth.Mappers;

public static class AuthMapper
{
    /// <summary>
    /// Maps token strings to the AuthResponse DTO.
    /// Used for Login and Refresh operations.
    /// </summary>
    public static AuthResponse ToResponse(string accessToken, string refreshToken)
    {
        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}