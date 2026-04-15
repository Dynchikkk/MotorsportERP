using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Cars;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class CarConfig : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        _ = builder.HasOne(c => c.Owner)
            .WithMany(u => u.Cars)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = builder.HasQueryFilter(e => !e.IsDeleted);
    }
}