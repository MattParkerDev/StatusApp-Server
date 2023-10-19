using Domain.Entities;
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
        friendship.Property(t => t.StatusUser1Id).HasConversion(x => x.Value, x => new(x));
        friendship.Property(t => t.StatusUser2Id).HasConversion(x => x.Value, x => new(x));

        friendship
            .HasOne<StatusUser>(s => s.StatusUser1)
            .WithMany(s => s.Friendships1)
            .HasForeignKey(s => s.StatusUser1Id);

        friendship
            .HasOne<StatusUser>(s => s.StatusUser2)
            .WithMany(s => s.Friendships2)
            .HasForeignKey(s => s.StatusUser2Id);
    }
}
