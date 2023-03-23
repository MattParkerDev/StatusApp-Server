using StatusApp.Server.Domain;

namespace StatusApp.Server.Application.Contracts;

public interface IFriendshipService
{
    Task<bool> AcceptFriendRequest(Friendship myFriendship, Friendship theirFriendship);
    Friendship? GetFriendship(string userName, Guid groupId);
    Friendship? GetFriendship(string userName, string friendUserName);
    List<Friendship> GetAllFriendships(string userName, bool? areFriends);
    List<string> GetFriendsUserNameList(string userName);
    Task<List<Profile>> GetFriendsProfileList(string userName);
    Task<Friendship?> CreateFriendshipPair(User user, User friendUser);
    Task<Friendship?> CreateAcceptedFriendshipPair(User user, User friendUser);

    Task<bool> RemoveFriendshipPair(Friendship myFriendship, Friendship theirFriendship);
}
