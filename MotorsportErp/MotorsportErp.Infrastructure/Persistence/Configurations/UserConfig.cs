using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MotorsportErp.Domain.Users;

namespace MotorsportErp.Infrastructure.Persistence.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        _ = builder.HasIndex(u => u.Email).IsUnique();

        _ = builder.Property(u => u.Bio).HasMaxLength(1000).IsRequired(false);
        _ = builder.Property(u => u.IsBlocked).HasDefaultValue(false);

        _ = builder.HasOne(u => u.Avatar)
           .WithMany()
           .HasForeignKey(u => u.AvatarId)
           .OnDelete(DeleteBehavior.SetNull);
    }
}