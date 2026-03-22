using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tournaments;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TournamentResultConfig : IEntityTypeConfiguration<TournamentResult>
{
    public void Configure(EntityTypeBuilder<TournamentResult> builder)
    {
        _ = builder.HasIndex(r => new { r.TournamentId, r.UserId }).IsUnique();

        _ = builder.HasOne(r => r.Tournament)
            .WithMany(t => t.Results)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasOne(r => r.User)
            .WithMany(u => u.Results)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}