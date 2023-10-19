using Domain.Common.Base;

namespace Domain.Entities;

public record ChatId(Guid Value);

public class Chat : BaseEntity<ChatId>
{
    public required string ChatName { get; set; }

    // Navigation properties
    public ICollection<StatusUser> ChatParticipants { get; set; } = new HashSet<StatusUser>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
    //public virtual ICollection<StatusUser> ChatAdmins { get; set; } = new HashSet<StatusUser>();
}
