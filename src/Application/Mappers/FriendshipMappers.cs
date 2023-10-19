using Application.DTOs;
using Domain.Entities;

namespace Application.Mappers;

public static class FriendshipMappers
{
    public static FriendshipDto ToDto(this Friendship friendship)
    {
        var dto = new FriendshipDto
        {
            Id = friendship.Id,
            UserName1 = friendship.StatusUser1!.UserName,
            UserName2 = friendship.StatusUser2!.UserName,
            UserName1Accepted = friendship.StatusUser1Accepted,
            UserName2Accepted = friendship.StatusUser2Accepted,
            BecameFriendsDate = friendship.BecameFriendsDate,
            ChatId = friendship.ChatId.Value
        };
        return dto;
    }
}
