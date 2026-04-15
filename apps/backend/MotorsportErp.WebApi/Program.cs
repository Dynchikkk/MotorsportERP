using Microsoft.EntityFrameworkCore;
using MotorsportErp.Application.Common.Settings;
using MotorsportErp.Infrastructure.Files;
using MotorsportErp.Infrastructure.Persistence;
using MotorsportErp.Infrastructure.Persistence.Settings;
using MotorsportErp.Infrastructure.Security;
using MotorsportErp.WebApi.Extensions;
using MotorsportErp.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<TrackSettings>(builder.Configuration.GetSection("TrackSettings"));
builder.Services.Configure<TournamentSettings>(builder.Configuration.GetSection("TournamentSettings"));
builder.Services.Configure<SeedSettings>(builder.Configuration.GetSection("SeedSettings"));
builder.Services.Configure<MediaFileStorageSettings>(builder.Configuration.GetSection("MediaFileStorageSettings"));

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            _ = sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        }));

// Services and infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure();

// JWT auth
builder.Services.AddJwtAuthentication(builder.Configuration);

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        _ = policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var initializer = services.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
    await initializer.SeedAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseFileStorage(builder.Configuration);

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

