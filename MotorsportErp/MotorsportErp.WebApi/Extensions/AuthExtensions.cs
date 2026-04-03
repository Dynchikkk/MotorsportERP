using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Auth;
using System.Text;

namespace MotorsportErp.WebApi.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var jwtSettings = config
            .GetSection("JwtSettings")
            .Get<JwtSettings>();

        _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        _ = services.AddAuthorizationBuilder()
            .AddPolicy("RequireOrganizer", policy =>
                policy.RequireRole(
                    nameof(UserRole.Organizer),
                    nameof(UserRole.Moderator),
                    nameof(UserRole.SuperAdmin)))
            .AddPolicy("RequireModerator", policy =>
                policy.RequireRole(
                    nameof(UserRole.Moderator),
                    nameof(UserRole.SuperAdmin)))
            .AddPolicy("RequireSuperAdmin", policy =>
                policy.RequireRole(nameof(UserRole.SuperAdmin)));

        return services;
    }
}