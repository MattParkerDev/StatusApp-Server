using Application.DTOs;
using Domain;

namespace Application.Mappers;

public static class StatusUserMappers
{
    public static StatusUserDto ToDto(this StatusUser statusUser)
    {
        var dto = new StatusUserDto
        {
            UserName = statusUser.UserName,
            FirstName = statusUser.FirstName,
            LastName = statusUser.LastName,
            Status = statusUser.Status,
            Online = statusUser.Online
        };
        return dto;
    }
}
