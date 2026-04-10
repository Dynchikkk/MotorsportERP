using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Files;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        _ = builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
