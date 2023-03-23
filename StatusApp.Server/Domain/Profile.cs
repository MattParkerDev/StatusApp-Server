using System.ComponentModel.DataAnnotations;

namespace StatusApp.Server.Domain;

public class Profile
{
    [Key]
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }
}
