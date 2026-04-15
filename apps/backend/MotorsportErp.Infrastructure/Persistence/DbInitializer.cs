using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MotorsportErp.Application.Common.Interfaces.Security;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Persistence.Settings;

namespace MotorsportErp.Infrastructure.Persistence;

public class DbInitializer
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SeedSettings _options;

    public DbInitializer(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IOptions<SeedSettings> options)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _options = options.Value;
    }

    /// <summary>
    /// Initialize db
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_context.Database.IsRelational())
        {
            await _context.Database.MigrateAsync();
        }
    }

    /// <summary>
    /// Seed data to db
    /// </summary>
    public async Task SeedAsync()
    {

        await SeedCoreDataAsync();

        if (_options.SeedDevelopmentData)
        {
            await SeedDevelopmentDataAsync();
        }
    }

    /// <summary>
    /// Core data seeding
    /// </summary>
    private async Task SeedCoreDataAsync()
    {
        const string adminEmail = "admin@motorsport.erp";

        if (!await _context.Users.AnyAsync(u => u.Email == adminEmail))
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Nickname = "SuperAdmin",
                Email = adminEmail,
                PasswordHash = _passwordHasher.Hash(_options.AdminPassword),
                Roles = UserRole.SuperAdmin | UserRole.Moderator | UserRole.Organizer | UserRole.Racer,
                IsBlocked = false,
                RaceCount = 0,
                Bio = "System Administrator"
            };

            _ = await _context.Users.AddAsync(admin);
            _ = await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Test data seeding
    /// </summary>
    private async Task SeedDevelopmentDataAsync()
    {
        var admin = await _context.Users.FirstAsync(u => u.Email == "admin@motorsport.erp");

        if (!await _context.Tracks.AnyAsync())
        {
            var track = new Track
            {
                Id = Guid.NewGuid(),
                Name = "Nürburgring Nordschleife",
                Location = "Nürburg, Germany",
                Status = TrackStatus.Official,
                ConfirmationThreshold = 10,
                CreatedById = admin.Id
            };

            _ = await _context.Tracks.AddAsync(track);
            _ = await _context.SaveChangesAsync();
        }
    }
}