using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class FriendshipService : IFriendshipService
{
    private readonly ChatContext _db;
    private readonly IUserService _userService;

    public FriendshipService(ChatContext db, IUserService userService)
    {
        _db = db;
        _userService = userService;
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

    public Friendship? GetFriendship(string userName, Guid groupId)
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

    public List<Friendship> GetAllFriendships(string userName, bool? areFriends)
    {
        List<Friendship> friendships;
        if (areFriends is null)
        {
            friendships = _db.Friendships.Where(s => s.UserName == userName).ToList();
            return friendships;
        }

        friendships = _db.Friendships
            .Where(s => s.UserName == userName && s.AreFriends == areFriends)
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
        var friendsProfileList = new List<Profile>();
        foreach (var name in friendUserNameList)
        {
            User? friend = await _userService.GetUserByNameAsync(name);
            if (friend == null)
            {
                //TODO: Review
                throw new ArgumentNullException();
            }
            friendsProfileList.Add(friend.ToProfile());
        }
        return friendsProfileList;
    }

    public async Task<Friendship?> CreateFriendshipPair(User user, User friendUser)
    {
        var myFriendship = new Friendship
        {
            UserName = user.UserName!,
            FriendUserName = friendUser.UserName!,
            Accepted = true,
            AreFriends = false,
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
        };
        var theirFriendship = new Friendship
        {
            UserName = friendUser.UserName!,
            FriendUserName = user.UserName!,
            Accepted = false,
            AreFriends = false,
            FriendFirstName = user.FirstName,
            FriendLastName = user.LastName,
        };
        _db.Friendships.Add(myFriendship);
        _db.Friendships.Add(theirFriendship);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return myFriendship;
    }

    public async Task<Friendship?> CreateAcceptedFriendshipPair(User user, User friendUser)
    {
        var guid = Guid.NewGuid();
        var time = DateTime.UtcNow;
        var myFriendship = new Friendship
        {
            UserName = user.UserName!,
            FriendUserName = friendUser.UserName!,
            Accepted = true,
            AreFriends = true,
            BecameFriendsDate = time,
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
            GroupId = guid
        };
        var theirFriendship = new Friendship
        {
            UserName = friendUser.UserName!,
            FriendUserName = user.UserName!,
            Accepted = true,
            AreFriends = true,
            BecameFriendsDate = time,
            FriendFirstName = user.FirstName,
            FriendLastName = user.LastName,
            GroupId = guid
        };
        _db.Friendships.Add(myFriendship);
        _db.Friendships.Add(theirFriendship);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return myFriendship;
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
