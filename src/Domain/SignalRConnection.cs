using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SignalRConnection
{
    [Key]
    public string? ConnectionId { get; set; }
    public string UserName { get; set; } = string.Empty;
}
