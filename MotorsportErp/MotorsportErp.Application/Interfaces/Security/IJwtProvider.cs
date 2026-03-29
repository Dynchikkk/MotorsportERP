using System.Security.Claims;

namespace MotorsportErp.Application.Interfaces.Security;

public interface IJwtProvider
{
    string GenerateToken(Domain.Users.User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}