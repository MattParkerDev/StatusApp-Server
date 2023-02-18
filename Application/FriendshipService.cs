using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class FriendshipService
{
    private readonly ChatContext _db;

    public FriendshipService(ChatContext db)
    {
        _db = db;
    }

    public async Task<bool> AcceptFriendRequest(Friendship myFriendship, Friendship theirFriendship)
    {
        var db = _db;
        var datetime = DateTime.UtcNow;
        var guid = Guid.NewGuid();
        myFriendship.Accepted = true;
        myFriendship.AreFriends = true;
        myFriendship.BecameFriendsDate = datetime;
        myFriendship.GroupId = guid;

        theirFriendship.AreFriends = true;
        theirFriendship.BecameFriendsDate = datetime;
        theirFriendship.GroupId = guid;
        try
        {
            await db.SaveChangesAsync();
        }
        catch
        {
            return false;
        }
        return true;
    }

    public Friendship? GetFriendship(Guid groupId, string userName)
    {
        var friendship = _db.Friendships.FirstOrDefault(
            s => s.GroupId == groupId && s.UserName == userName
        );
        return friendship;
    }

    public Friendship? GetFriendship(string userName, string friendUserName)
    {
        var friendship = _db.Friendships.FirstOrDefault(
            s => s.UserName == userName && s.FriendUserName == friendUserName
        );
        return friendship;
    }

    public async Task<bool> RemoveFriendshipPair(
        Friendship myFriendship,
        Friendship theirFriendship
    )
    {
        _db.Friendships.Remove(myFriendship);
        _db.Friendships.Remove(theirFriendship);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return false;
        }
        return true;
    }
}
