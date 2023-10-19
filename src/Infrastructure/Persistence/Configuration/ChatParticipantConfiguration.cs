using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> chatParticipant)
    {
        chatParticipant.HasKey(t => t.Id);
        chatParticipant.Property(s => s.Id).ValueGeneratedOnAdd();
        chatParticipant.Property(t => t.ChatId).HasConversion(x => x.Value, x => new ChatId(x));

        chatParticipant.HasOne(s => s.Chat).WithMany().HasForeignKey(s => s.ChatId);

        chatParticipant.HasOne(s => s.StatusUser).WithMany().HasForeignKey(s => s.UserName);
    }
}
