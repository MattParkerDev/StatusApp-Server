﻿namespace Domain.Entities;

public class Message
{
    // TODO: Guid for Id and autoincrement int Sequence
    public int? Id { get; set; }
    public string AuthorUserName { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; }

    // Foreign keys
    public required ChatId ChatId { get; set; }
    public required StatusUserId StatusUserId { get; set; }

    // Navigation properties
    public Chat Chat { get; set; } = null!;
    public StatusUser StatusUser { get; set; } = null!;
}
