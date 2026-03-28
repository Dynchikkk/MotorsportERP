using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Application.Interfaces.Security;
using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Services;
using MotorsportErp.Infrastructure.Auth;
using MotorsportErp.Infrastructure.Repositories;

namespace MotorsportErp.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IAuthService, AuthService>();
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<ICarService, CarService>();
        _ = services.AddScoped<ITrackService, TrackService>();
        _ = services.AddScoped<ITournamentService, TournamentService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<ICarRepository, CarRepository>();
        _ = services.AddScoped<ITrackRepository, TrackRepository>();
        _ = services.AddScoped<ITournamentRepository, TournamentRepository>();
        _ = services.AddScoped<ITournamentApplicationRepository, TournamentApplicationRepository>();

        _ = services.AddScoped<IJwtProvider, JwtProvider>();
        _ = services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}