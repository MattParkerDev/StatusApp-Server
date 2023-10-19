using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class SignalRConnection
{
    [Key]
    public string? ConnectionId { get; set; }
    public string UserName { get; set; } = string.Empty;

    // Navigation properties
    public StatusUser StatusUser { get; set; } = null!;
}
