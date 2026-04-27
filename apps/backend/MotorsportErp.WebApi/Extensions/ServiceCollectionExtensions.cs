using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Interfaces.Files;
using MotorsportErp.Application.Common.Interfaces.Repositories;
using MotorsportErp.Application.Common.Interfaces.Security;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Application.Features.Auth.Interfaces;
using MotorsportErp.Application.Features.Auth.Services;
using MotorsportErp.Application.Features.Cars.Interfaces;
using MotorsportErp.Application.Features.Cars.Services;
using MotorsportErp.Application.Features.MediaFiles.Interfaces;
using MotorsportErp.Application.Features.MediaFiles.Services;
using MotorsportErp.Application.Features.Tournaments.Interfaces;
using MotorsportErp.Application.Features.Tournaments.Services;
using MotorsportErp.Application.Features.Tracks.Interfaces;
using MotorsportErp.Application.Features.Tracks.Services;
using MotorsportErp.Application.Features.Users.Interfaces;
using MotorsportErp.Application.Features.Users.Services;
using MotorsportErp.Infrastructure.Files;
using MotorsportErp.Infrastructure.Persistence;
using MotorsportErp.Infrastructure.Persistence.Repositories;
using MotorsportErp.Infrastructure.Persistence.Settings;
using MotorsportErp.Infrastructure.Security;

namespace MotorsportErp.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        _ = services.Configure<TrackSettings>(configuration.GetSection("TrackSettings"));
        _ = services.Configure<TournamentSettings>(configuration.GetSection("TournamentSettings"));
        _ = services.Configure<SeedSettings>(configuration.GetSection("SeedSettings"));

        _ = services.AddSingleton<IValidateOptions<MediaFileStorageSettings>, MediaFileStorageSettingsValidator>();
        _ = services
            .AddOptions<MediaFileStorageSettings>()
            .Bind(configuration.GetSection("MediaFileStorageSettings"))
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        _ = services.AddScoped<IAuthService, AuthService>();
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<ICarService, CarService>();
        _ = services.AddScoped<ITrackService, TrackService>();
        _ = services.AddScoped<ITournamentService, TournamentService>();
        _ = services.AddScoped<IMediaFileService, MediaFileService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<ICarRepository, CarRepository>();
        _ = services.AddScoped<ITrackRepository, TrackRepository>();
        _ = services.AddScoped<ITournamentRepository, TournamentRepository>();
        _ = services.AddScoped<ITournamentApplicationRepository, TournamentApplicationRepository>();
        _ = services.AddScoped<IFileRepository, FileRepository>();

        _ = services.AddScoped<IJwtProvider, JwtProvider>();
        _ = services.AddScoped<IPasswordHasher, PasswordHasher>();
        _ = services.AddScoped<IMediaFileStorageProvider, LocalMediaFileStorageProvider>();

        _ = services.AddScoped<DbInitializer>();

        return services;
    }
}
