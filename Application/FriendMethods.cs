using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public static class FriendMethods
{
    public static List<User> GetFriends(ChatContext db, int accountId)
    {
        var friendships = GetFriendships(db, accountId);

        var friendIdList = GetFriendIdList(friendships);
        var friends = db.Users.Where(s => friendIdList.Contains(s.AccountId)).ToList();
        return friends;
    }

    public static List<int> GetFriendIdList(List<Friendship> friendships)
    {
        var friendIdList = new List<int>();
        foreach (var item in friendships)
        {
            friendIdList.Add(item.FriendId);
        }

        return friendIdList;
    }

    public static List<Friendship> GetFriendships(ChatContext db, int accountId)
    {
        var friendships = db.Friendships
            .Where(s => s.AccountId == accountId && s.AreFriends == true)
            .ToList();
        return friendships;
    }
}