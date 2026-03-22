using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TournamentConfig : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        _ = builder.HasOne(t => t.Track)
            .WithMany(tr => tr.Tournaments)
            .HasForeignKey(t => t.TrackId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTournaments)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}