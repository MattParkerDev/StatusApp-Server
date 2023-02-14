using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public static class FriendMethods
{
    public static async Task<List<Profile>> GetFriends(
        ChatContext db,
        UserManager<User> userManager,
        string userName
    )
    {
        var friendships = GetFriendships(db, userName);
        var friendUserNameList = GetFriendUserNameList(friendships);
        List<Profile> friendUsers = new List<Profile>();
        foreach (var name in friendUserNameList)
        {
            User? friend = await userManager.FindByNameAsync(name);
            if (friend == null)
            {
                //TODO: Review
                throw new ArgumentNullException();
            }
            friendUsers.Add(friend.ToProfile());
        }
        return friendUsers;
    }

    public static List<string> GetFriendUserNameList(List<Friendship> friendships)
    {
        var friendIdList = new List<string>();
        foreach (var item in friendships)
        {
            friendIdList.Add(item.FriendUserName);
        }

        return friendIdList;
    }

    // Returns all friendships associated with the passed userName
    public static List<Friendship> GetFriendships(ChatContext db, string userName)
    {
        var friendships = db.Friendships
            .Where(s => s.UserName == userName && s.AreFriends == true)
            .ToList();
        return friendships;
    }
}
