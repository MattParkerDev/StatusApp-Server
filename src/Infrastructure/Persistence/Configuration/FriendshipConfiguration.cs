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

        friendship
            .HasOne<StatusUser>(s => s.StatusUser1)
            .WithMany()
            .HasForeignKey(s => s.UserName1);

        friendship
            .HasOne<StatusUser>(s => s.StatusUser2)
            .WithMany()
            .HasForeignKey(s => s.UserName2);
    }
}
