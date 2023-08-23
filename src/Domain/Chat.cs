using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Chat
{
    [Key]
    public required Guid Id { get; set; }
    public required string ChatName { get; set; }
    public virtual ICollection<StatusUser> ChatParticipants { get; set; } =
        new HashSet<StatusUser>();
    public virtual ICollection<StatusUser> ChatAdmins { get; set; } = new HashSet<StatusUser>();
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
