using Domain;
using Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> chat)
    {
        chat.HasKey(t => t.Id);

        chat.Property(t => t.Id)
            .HasConversion(x => x.Value, x => new ChatId(x))
            .ValueGeneratedOnAdd();

        chat.HasMany<StatusUser>(t => t.ChatParticipants)
            .WithMany(t => t.Chats)
            .UsingEntity<ChatParticipant>();
    }
}
