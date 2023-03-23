using System.ComponentModel.DataAnnotations;

namespace StatusApp.Server.Domain;

public class Account
{
    //Review and delete
    [Key]
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
}
