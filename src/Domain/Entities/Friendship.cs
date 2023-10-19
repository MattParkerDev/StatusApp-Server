namespace Domain.Entities;

public class Friendship
{
    public Guid Id { get; set; }
    public bool StatusUser1Accepted { get; set; } = false;
    public bool StatusUser2Accepted { get; set; } = false;
    public DateTime BecameFriendsDate { get; set; }

    // Foreign keys
    public ChatId? ChatId { get; set; } = null!;
    public required StatusUserId StatusUser1Id { get; set; }
    public required StatusUserId StatusUser2Id { get; set; }

    // Navigation properties
    public virtual Chat? Chat { get; set; } = null!;
    public virtual StatusUser? StatusUser1 { get; set; } = null!;
    public virtual StatusUser? StatusUser2 { get; set; } = null!;
}
