using Microsoft.EntityFrameworkCore;
using MotorsportErp.Infrastructure.Persistence;
using MotorsportErp.Infrastructure.Security;
using MotorsportErp.WebApi.Extensions;
using MotorsportErp.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.AddApplicationOptions(builder.Configuration);

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

app.UseFileStorage();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

