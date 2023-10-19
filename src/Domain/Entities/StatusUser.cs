using Domain.Common.Base;

namespace Domain.Entities;

public record StatusUserId(Guid Value);

public class StatusUser : BaseEntity<StatusUserId>
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new HashSet<Chat>();
    public virtual ICollection<Friendship> Friendships1 { get; set; } = new HashSet<Friendship>();
    public virtual ICollection<Friendship> Friendships2 { get; set; } = new HashSet<Friendship>();
}
