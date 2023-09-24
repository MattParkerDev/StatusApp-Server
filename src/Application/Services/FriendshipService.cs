using Application.DTOs;
using Application.Mappers;
using Application.Services.Contracts;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

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

        var chat = new Chat
        {
            ChatName = "New Chat",
            ChatParticipants = new List<StatusUser>()
            {
                friendship.StatusUser1,
                friendship.StatusUser2
            }
        };
        _db.Chats.Add(chat);

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
        var friendship = _db.Friendships
            .Include(x => x.StatusUser1)
            .Include(y => y.StatusUser2)
            .FirstOrDefault(
                s =>
                    (s.UserName1 == userName || s.UserName1 == friendUserName)
                    && (s.UserName2 == userName || s.UserName2 == friendUserName)
            );
        return friendship;
    }

    public List<Friendship> GetAllFriendships(string userName, bool? areFriends)
    {
        var friendships = areFriends switch
        {
            true
                => _db.Friendships
                    .Where(s => s.UserName1 == userName || s.UserName2 == userName)
                    .Where(x => x.UserName1Accepted == true && x.UserName2Accepted == true)
                    .Include(x => x.StatusUser1)
                    .Include(y => y.StatusUser2)
                    .Include(z => z.Chat)
                    .ToList(),
            false
                => _db.Friendships
                    .Where(s => s.UserName1 == userName || s.UserName2 == userName)
                    .Where(x => x.UserName1Accepted == false || x.UserName2Accepted == false)
                    .Include(x => x.StatusUser1)
                    .Include(y => y.StatusUser2)
                    .Include(z => z.Chat)
                    .ToList(),
            null
                => _db.Friendships
                    .Where(s => s.UserName1 == userName || s.UserName2 == userName)
                    .Include(x => x.StatusUser1)
                    .Include(y => y.StatusUser2)
                    .Include(z => z.Chat)
                    .ToList(),
        };
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
        var friendsDtoList = new List<StatusUserDto>();

        friendsDtoList = await _db.StatusUsers
            .Where(
                s =>
                    s.Friendships1.Any(
                        x =>
                            (x.UserName1 == userName || x.UserName2 == userName)
                            && x.UserName1Accepted == true
                            && x.UserName2Accepted == true
                    )
            )
            .Select(x => x.ToDto())
            .ToListAsync();

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

        var chat = new Chat
        {
            ChatName = "New Chat",
            ChatParticipants = new List<StatusUser>() { user, friendUser }
        };

        var friendship = new Friendship
        {
            UserName1 = user.UserName,
            UserName2 = friendUser.UserName,
            UserName1Accepted = true,
            UserName2Accepted = true,
            BecameFriendsDate = time,
            Chat = chat
        };

        _db.Friendships.Add(friendship);
        _db.Chats.Add(chat);

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
