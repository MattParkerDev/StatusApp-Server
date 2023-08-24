using Domain;
using Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class StatusUserConfiguration : IEntityTypeConfiguration<StatusUser>
{
    public void Configure(EntityTypeBuilder<StatusUser> statusUser)
    {
        statusUser.HasKey(t => t.UserName);

        statusUser.HasMany<Friendship>().WithOne().HasForeignKey(s => s.UserName1);
        statusUser.HasMany<Friendship>().WithOne().HasForeignKey(s => s.UserName2);
    }
}
