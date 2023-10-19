using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class StatusUserConfiguration : IEntityTypeConfiguration<StatusUser>
{
    public void Configure(EntityTypeBuilder<StatusUser> statusUser)
    {
        statusUser.HasKey(t => t.Id);
        statusUser.HasIndex(t => t.UserName);

        statusUser.Property(t => t.Id).HasConversion(x => x.Value, x => new StatusUserId(x));

        statusUser
            .HasMany<Friendship>(e => e.Friendships1)
            .WithOne(s => s.StatusUser1)
            .HasForeignKey(x => x.StatusUser1Id);
        statusUser
            .HasMany<Friendship>(e => e.Friendships2)
            .WithOne(s => s.StatusUser2)
            .HasForeignKey(x => x.StatusUser2Id);
    }
}
