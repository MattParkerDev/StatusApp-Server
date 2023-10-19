using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class SignalRConnection
{
    public string? ConnectionId { get; set; }
    public StatusUserId? StatusUserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    // Navigation properties
    public StatusUser? StatusUser { get; set; } = null!;
}
