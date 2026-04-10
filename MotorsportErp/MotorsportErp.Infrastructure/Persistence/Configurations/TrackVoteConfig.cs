using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Tracks;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class TrackVoteConfig : IEntityTypeConfiguration<TrackVote>
{
    public void Configure(EntityTypeBuilder<TrackVote> builder)
    {
        _ = builder.HasIndex(v => new { v.UserId, v.TrackId }).IsUnique().HasFilter("[IsDeleted] = 0"); ;

        _ = builder.HasOne(v => v.User)
            .WithMany(u => u.TrackVotes)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasOne(v => v.Track)
            .WithMany(t => t.Votes)
            .HasForeignKey(v => v.TrackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}