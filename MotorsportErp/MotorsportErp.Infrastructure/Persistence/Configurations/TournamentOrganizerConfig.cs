using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TournamentOrganizerConfig : IEntityTypeConfiguration<TournamentOrganizer>
{
    public void Configure(EntityTypeBuilder<TournamentOrganizer> builder)
    {
        _ = builder.HasIndex(o => new { o.TournamentId, o.UserId }).IsUnique();

        _ = builder.HasOne(o => o.Tournament)
            .WithMany(t => t.Organizers)
            .HasForeignKey(o => o.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasOne(o => o.User)
            .WithMany(u => u.OrganizedTournaments)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}