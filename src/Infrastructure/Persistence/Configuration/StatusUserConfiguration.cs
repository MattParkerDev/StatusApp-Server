using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class StatusUserConfiguration : IEntityTypeConfiguration<StatusUser>
{
    public void Configure(EntityTypeBuilder<StatusUser> statusUser)
    {
        statusUser.HasKey(t => t.UserName);

        statusUser
            .HasMany<Friendship>()
            .WithOne(s => s.StatusUser1)
            .HasForeignKey(x => x.UserName1);
        statusUser
            .HasMany<Friendship>()
            .WithOne(s => s.StatusUser2)
            .HasForeignKey(x => x.UserName2);
    }
}
