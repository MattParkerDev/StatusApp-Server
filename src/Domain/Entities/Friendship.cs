namespace Domain.Entities;

public class Friendship
{
    public Guid Id { get; set; }
    public bool UserName1Accepted { get; set; } = false;
    public bool UserName2Accepted { get; set; } = false;
    public DateTime BecameFriendsDate { get; set; }

    // Foreign keys
    public ChatId? ChatId { get; set; } = null!;
    public string UserName1 { get; set; } = string.Empty;
    public string UserName2 { get; set; } = string.Empty;

    // Navigation properties
    public virtual Chat? Chat { get; set; } = null!;
    public virtual StatusUser? StatusUser1 { get; set; } = null!;
    public virtual StatusUser? StatusUser2 { get; set; } = null!;
}
