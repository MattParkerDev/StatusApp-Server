using System.ComponentModel.DataAnnotations;

namespace StatusApp.Server.Domain;

public class StatusUser
{
    [Key]
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }

    public Profile ToProfile()
    {
        var profile = new Profile
        {
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            Status = Status,
            Online = Online
        };
        return profile;
    }
}
