using MotorsportErp.Application.Interfaces.Services;
using MotorsportErp.Application.Services;
using MotorsportErp.Application.Interfaces.Repositories;
using MotorsportErp.Infrastructure.Repositories;
using MotorsportErp.Application.Interfaces.Security;
using MotorsportErp.Infrastructure.Auth;

namespace MotorsportErp.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICarService, CarService>();
        services.AddScoped<ITrackService, TrackService>();
        services.AddScoped<ITournamentService, TournamentService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICarRepository, CarRepository>();
        services.AddScoped<ITrackRepository, TrackRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<ITournamentApplicationRepository, TournamentApplicationRepository>();

        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        return services;
    }
}