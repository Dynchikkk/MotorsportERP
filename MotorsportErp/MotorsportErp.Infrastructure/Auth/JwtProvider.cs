using Microsoft.IdentityModel.Tokens;
using MotorsportErp.Application.Interfaces.Security;
using MotorsportErp.Domain.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MotorsportErp.Infrastructure.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _settings;

    public JwtProvider(JwtSettings settings)
    {
        _settings = settings;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
        {
            if (role == UserRole.None)
            {
                continue;
            }

            if (user.Roles.HasFlag(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}