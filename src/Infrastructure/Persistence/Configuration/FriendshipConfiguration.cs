using Domain;
using Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> friendship)
    {
        friendship.HasKey(t => t.Id);
        friendship.HasOne<Chat>(t => t.Chat);

        friendship.Property(t => t.ChatId).HasConversion(x => x.Value, x => new ChatId(x));

        friendship
            .HasOne<StatusUser>()
            .WithMany(s => s.Friendships)
            .HasForeignKey(t => t.UserName1);

        friendship
            .HasOne<StatusUser>()
            .WithMany(s => s.Friendships)
            .HasForeignKey(s => s.UserName2);
    }
}
