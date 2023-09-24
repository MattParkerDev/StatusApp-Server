using Application.DTOs;
using Domain;

namespace Application.Mappers;

public static class FriendshipMappers
{
    public static FriendshipDto ToDto(this Friendship friendship)
    {
        var dto = new FriendshipDto
        {
            Id = friendship.Id,
            UserName1 = friendship.UserName1,
            UserName2 = friendship.UserName2,
            UserName1Accepted = friendship.UserName1Accepted,
            UserName2Accepted = friendship.UserName2Accepted,
            BecameFriendsDate = friendship.BecameFriendsDate,
            ChatId = friendship.ChatId.Value
        };
        return dto;
    }
}
