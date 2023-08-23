using Domain;

namespace Application.DTOs;

public class StatusUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }

    // TODO: Review, not best practice?
    public StatusUser FromDto()
    {
        var statusUser = new StatusUser
        {
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            Status = Status,
            Online = Online
        };
        return statusUser;
    }
}
