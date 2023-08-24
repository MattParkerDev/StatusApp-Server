using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Friendship
{
    public Guid Id { get; set; }
    public bool UserName1Accepted { get; set; } = false;
    public bool UserName2Accepted { get; set; } = false;
    public DateTime BecameFriendsDate { get; set; }

    // Foreign keys
    public string UserName1 { get; set; } = string.Empty;
    public string UserName2 { get; set; } = string.Empty;
    public ChatId? ChatId { get; set; } = null!;

    // Navigation properties
    public virtual Chat? Chat { get; set; } = null!;
}
