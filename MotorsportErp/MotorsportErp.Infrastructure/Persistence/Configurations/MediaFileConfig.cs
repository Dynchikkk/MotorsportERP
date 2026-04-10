using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Cars;
using MotorsportErp.Domain.Files;
using MotorsportErp.Domain.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class MediaFileConfig : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
