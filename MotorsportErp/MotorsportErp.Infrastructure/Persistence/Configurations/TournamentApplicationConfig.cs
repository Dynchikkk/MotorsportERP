using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TournamentApplicationConfig : IEntityTypeConfiguration<TournamentApplication>
{
    public void Configure(EntityTypeBuilder<TournamentApplication> builder)
    {
        _ = builder.HasIndex(a => new { a.UserId, a.TournamentId }).IsUnique().HasFilter("[IsDeleted] = 0");

        _ = builder.HasOne(a => a.User)
            .WithMany(u => u.Applications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasOne(a => a.Tournament)
            .WithMany(t => t.Applications)
            .HasForeignKey(a => a.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasOne(a => a.Car)
            .WithMany(c => c.Applications)
            .HasForeignKey(a => a.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasQueryFilter(e => !e.IsDeleted);
    }
}