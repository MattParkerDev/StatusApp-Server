using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class ChatMappers
{
    public static ChatDto ToDto(this Chat chat)
    {
        var dto = new ChatDto
        {
            Id = chat.Id.Value,
            ChatName = chat.ChatName,
            ChatParticipants = chat.ChatParticipants.Select(x => x.ToDto()).ToList()
        };
        return dto;
    }
}
