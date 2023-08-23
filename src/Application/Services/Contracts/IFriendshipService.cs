using Application.DTOs;
using Domain;

namespace Application.Services.Contracts;

public interface IFriendshipService
{
    Task<bool> AcceptFriendRequest(Friendship myFriendship);
    Friendship? GetFriendship(Guid friendshipId);
    Friendship? GetFriendship(string userName, string friendUserName);
    List<Friendship> GetAllFriendships(string userName, bool? areFriends);
    List<string> GetFriendsUserNameList(string userName);
    Task<List<StatusUserDto>> GetFriendsDtoList(string userName);
    Task<Friendship?> CreateFriendship(StatusUser user, StatusUser friendUser);
    Task<Friendship?> CreateAcceptedFriendship(StatusUser user, StatusUser friendUser);
    Task<bool> DeleteFriendship(Friendship myFriendship);
}
