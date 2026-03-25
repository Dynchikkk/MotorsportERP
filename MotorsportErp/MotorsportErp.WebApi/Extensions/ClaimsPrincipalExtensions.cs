using System.Security.Claims;

namespace MotorsportErp.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Invalid user id");

        return userId;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        var email = (user.FindFirst(ClaimTypes.Email)?.Value) ?? throw new UnauthorizedAccessException("Email not found in token");
        return email;
    }

    public static string GetRole(this ClaimsPrincipal user)
    {
        var role = (user.FindFirst(ClaimTypes.Role)?.Value) ?? throw new UnauthorizedAccessException("Role not found in token");
        return role;
    }
}