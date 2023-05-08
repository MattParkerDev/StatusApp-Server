using System.ComponentModel.DataAnnotations;
using Domain.DTOs;

namespace Domain;

public class StatusUser
{
    [Key]
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }

    public StatusUserDto ToDto()
    {
        var dto = new StatusUserDto
        {
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            Status = Status,
            Online = Online
        };
        return dto;
    }
}
