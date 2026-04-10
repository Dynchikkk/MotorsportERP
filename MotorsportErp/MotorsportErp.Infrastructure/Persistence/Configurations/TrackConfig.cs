using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TrackConfig : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        _ = builder.HasOne(t => t.CreatedBy)
            .WithMany(u => u.CreatedTracks)
            .HasForeignKey(t => t.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasQueryFilter(e => !e.IsDeleted);
    }
}