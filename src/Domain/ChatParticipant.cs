namespace Domain;

public class ChatParticipant
{
    public Guid Id { get; set; }

    // Foreign keys
    public required ChatId ChatId { get; set; }
    public required string UserName { get; set; }
    public bool IsAdmin { get; set; } = false;

    // Navigation properties
    public Chat Chat { get; set; } = null!;
    public StatusUser StatusUser { get; set; } = null!;
}
