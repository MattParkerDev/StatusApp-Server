using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class MessageMappers
{
    public static MessageDto ToDto(this Message message)
    {
        var dto = new MessageDto
        {
            Id = message.Id.Value,
            AuthorUserName = message.AuthorUserName,
            Data = message.Data,
            Created = message.Created,
            LastUpdated = message.LastUpdated,
            ChatId = message.ChatId.Value
        };
        return dto;
    }
}
