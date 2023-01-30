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

    public async Task AcceptFriendRequest(Friendship myFriendship, Friendship theirFriendship)
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

        await db.SaveChangesAsync();
    }

    public Friendship? GetFriendship(Guid groupId, string userName)
    {
        var friendship = _db.Friendships.FirstOrDefault(
            s => s.GroupId == groupId && s.UserName == userName
        );
        return friendship;
    }
}
