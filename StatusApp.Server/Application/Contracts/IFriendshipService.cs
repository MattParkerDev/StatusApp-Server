using StatusApp.Server.Domain;
using StatusApp.Server.Domain.DTOs;

namespace StatusApp.Server.Application.Contracts;

public interface IFriendshipService
{
    Task<bool> AcceptFriendRequest(Friendship myFriendship, Friendship theirFriendship);
    Friendship? GetFriendship(string userName, Guid groupId);
    Friendship? GetFriendship(string userName, string friendUserName);
    List<Friendship> GetAllFriendships(string userName, bool? areFriends);
    List<string> GetFriendsUserNameList(string userName);
    Task<List<StatusUserDto>> GetFriendsDtoList(string userName);
    Task<Friendship?> CreateFriendshipPair(StatusUser user, StatusUser friendUser);
    Task<Friendship?> CreateAcceptedFriendshipPair(StatusUser user, StatusUser friendUser);
    Task<bool> DeleteFriendshipPair(Friendship myFriendship, Friendship theirFriendship);
}
