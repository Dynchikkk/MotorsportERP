using Microsoft.EntityFrameworkCore;
using MotorsportErp.Domain.BaseEntities;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Tournaments;
using MotorsportErp.Domain.Tracks;
using MotorsportErp.Domain.Users;
using MotorsportErp.Infrastructure.Persistence.Configurations;

namespace MotorsportErp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<Track> Tracks => Set<Track>();
    public DbSet<TrackVote> TrackVotes => Set<TrackVote>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentApplication> TournamentApplications => Set<TournamentApplication>();
    public DbSet<TournamentResult> TournamentResults => Set<TournamentResult>();
    public DbSet<TournamentOrganizer> TournamentOrganizers => Set<TournamentOrganizer>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfiguration(new UserConfig());
        _ = modelBuilder.ApplyConfiguration(new CarConfig());
        _ = modelBuilder.ApplyConfiguration(new TrackConfig());
        _ = modelBuilder.ApplyConfiguration(new TrackVoteConfig());
        _ = modelBuilder.ApplyConfiguration(new TournamentConfig());
        _ = modelBuilder.ApplyConfiguration(new TournamentApplicationConfig());
        _ = modelBuilder.ApplyConfiguration(new TournamentResultConfig());
        _ = modelBuilder.ApplyConfiguration(new TournamentOrganizerConfig());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is GuidEntity && (
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                ((GuidEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}