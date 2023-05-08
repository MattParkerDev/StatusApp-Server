using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Connection
{
    public string UserName { get; set; } = string.Empty;

    [Key]
    public string? ConnectionId { get; set; }
}
