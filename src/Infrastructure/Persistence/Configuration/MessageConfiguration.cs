using Domain;
using Domain.Common.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> message)
    {
        message.HasKey(t => t.Id);
        message.Property(s => s.Id).ValueGeneratedOnAdd();

        message.Property(t => t.ChatId).HasConversion(x => x.Value, x => new ChatId(x));

        message
            .HasOne<StatusUser>(s => s.StatusUser)
            .WithMany()
            .HasForeignKey(s => s.AuthorUserName);

        message.HasOne<Chat>(s => s.Chat).WithMany(s => s.Messages).HasForeignKey(s => s.ChatId);
    }
}
