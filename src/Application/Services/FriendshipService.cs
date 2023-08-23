using Application.Contracts;
using Application.DTOs;
using Application.Mappers;
using Application.Services.Contracts;
using Domain;

namespace Application;

public class FriendshipService : IFriendshipService
{
    private readonly IStatusContext _db;
    private readonly IStatusUserService _statusUserService;

    public FriendshipService(IStatusContext db, IStatusUserService statusUserService)
    {
        _db = db;
        _statusUserService = statusUserService;
    }

    public async Task<bool> AcceptFriendRequest(Friendship friendship)
    {
        var datetime = DateTime.UtcNow;
        friendship.UserName1Accepted = true;
        friendship.UserName2Accepted = true;
        friendship.BecameFriendsDate = datetime;

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

    public Friendship? GetFriendship(Guid friendshipId)
    {
        var friendship = _db.Friendships.FirstOrDefault(s => s.Id == friendshipId);
        return friendship;
    }

    public Friendship? GetFriendship(string userName, string friendUserName)
    {
        var friendship = _db.Friendships.FirstOrDefault(
            s =>
                (s.UserName1 == userName || s.UserName1 == friendUserName)
                && (s.UserName2 == userName || s.UserName2 == friendUserName)
        );
        return friendship;
    }

    public List<Friendship> GetAllFriendships(string userName, bool? areFriends)
    {
        List<Friendship> friendships;
        if (areFriends is null)
        {
            friendships = _db.Friendships
                .Where(s => s.UserName1 == userName || s.UserName2 == userName)
                .ToList();
            return friendships;
        }

        friendships = _db.Friendships
            .Where(s => s.UserName1 == userName || s.UserName2 == userName)
            .Where(x => x.UserName1Accepted == areFriends && x.UserName2Accepted == areFriends)
            .ToList();
        return friendships;
    }

    public List<string> GetFriendsUserNameList(string userName)
    {
        var friendUserNameList = _db.Friendships
            .Where(s => s.UserName1 == userName && s.UserName2Accepted == true)
            .Select(x => x.UserName2)
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
                throw new ArgumentNullException(nameof(statusUser));
            }
            friendsDtoList.Add(statusUser.ToDto());
        }
        return friendsDtoList;
    }

    public async Task<Friendship?> CreateFriendship(StatusUser user, StatusUser friendUser)
    {
        var friendship = new Friendship
        {
            UserName1 = user.UserName,
            UserName2 = friendUser.UserName,
            UserName1Accepted = true,
            UserName2Accepted = false,
        };

        _db.Friendships.Add(friendship);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return friendship;
    }

    public async Task<Friendship?> CreateAcceptedFriendship(StatusUser user, StatusUser friendUser)
    {
        var guid = Guid.NewGuid();
        var time = DateTime.UtcNow;
        var friendship = new Friendship
        {
            UserName1 = user.UserName,
            UserName2 = friendUser.UserName,
            UserName1Accepted = true,
            UserName2Accepted = true,
            BecameFriendsDate = time,
        };

        _db.Friendships.Add(friendship);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return friendship;
    }

    public async Task<bool> DeleteFriendship(Friendship myFriendship)
    {
        _db.Friendships.Remove(myFriendship);
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
