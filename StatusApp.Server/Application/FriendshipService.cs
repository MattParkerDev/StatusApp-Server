using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Domain.DTOs;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Application;

public class FriendshipService : IFriendshipService
{
    private readonly StatusContext _db;
    private readonly IStatusUserService _statusUserService;

    public FriendshipService(StatusContext db, IStatusUserService statusUserService)
    {
        _db = db;
        _statusUserService = statusUserService;
    }

    public async Task<bool> AcceptFriendRequest(Friendship myFriendship, Friendship theirFriendship)
    {
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
            await _db.SaveChangesAsync();
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

    public async Task<List<StatusUserDto>> GetFriendsDtoList(string userName)
    {
        var friendUserNameList = GetFriendsUserNameList(userName);
        var friendsDtoList = new List<StatusUserDto>();
        foreach (var name in friendUserNameList)
        {
            var statusUser = await _statusUserService.GetUserByNameAsync(name);
            if (statusUser == null)
            {
                //TODO: Review
                throw new ArgumentNullException();
            }
            friendsDtoList.Add(statusUser.ToDto());
        }
        return friendsDtoList;
    }

    public async Task<Friendship?> CreateFriendshipPair(StatusUser user, StatusUser friendUser)
    {
        var myFriendship = new Friendship
        {
            UserName = user.UserName,
            FriendUserName = friendUser.UserName,
            Accepted = true,
            AreFriends = false,
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
        };
        var theirFriendship = new Friendship
        {
            UserName = friendUser.UserName,
            FriendUserName = user.UserName,
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

    public async Task<Friendship?> CreateAcceptedFriendshipPair(
        StatusUser user,
        StatusUser friendUser
    )
    {
        var guid = Guid.NewGuid();
        var time = DateTime.UtcNow;
        var myFriendship = new Friendship
        {
            UserName = user.UserName,
            FriendUserName = friendUser.UserName,
            Accepted = true,
            AreFriends = true,
            BecameFriendsDate = time,
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
            GroupId = guid
        };
        var theirFriendship = new Friendship
        {
            UserName = friendUser.UserName,
            FriendUserName = user.UserName,
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

    public async Task<bool> DeleteFriendshipPair(
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
