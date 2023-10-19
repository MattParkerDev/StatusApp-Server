using Application.DTOs;
using Domain.Entities;

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

    public static StatusUser FromDto(this StatusUserDto statusUserDto)
    {
        var statusUser = new StatusUser
        {
            UserName = statusUserDto.UserName,
            FirstName = statusUserDto.FirstName,
            LastName = statusUserDto.LastName,
            Status = statusUserDto.Status,
            Online = statusUserDto.Online
        };
        return statusUser;
    }
}
