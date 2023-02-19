using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class FriendshipService
{
    private readonly ChatContext _db;
    private readonly UserManager<User> _userManager;

    public FriendshipService(ChatContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
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

    public List<Friendship> GetAllFriendships(string userName)
    {
        var friendships = _db.Friendships
            .Where(s => s.UserName == userName && s.AreFriends == true)
            .ToList();
        return friendships;
    }

    public List<string> GetFriendsUserNameList(string userName)
    {
        var friendUserNameList = _db.Friendships
            .Where(s => s.UserName == userName && s.AreFriends == true)
            .Select(x => x.FriendUserName)
            .ToList();

        return friendUserNameList;
    }

    public async Task<List<Profile>> GetFriendsProfileList(string userName)
    {
        var friendUserNameList = GetFriendsUserNameList(userName);
        List<Profile> friendsProfileList = new List<Profile>();
        foreach (var name in friendUserNameList)
        {
            User? friend = await _userManager.FindByNameAsync(name);
            if (friend == null)
            {
                //TODO: Review
                throw new ArgumentNullException();
            }
            friendsProfileList.Add(friend.ToProfile());
        }
        return friendsProfileList;
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
