using Microsoft.EntityFrameworkCore;

namespace StatusApp_Server.Domain
{
    [PrimaryKey(nameof(AccountId), nameof(FriendId))]
    public class Friendship
    {
        public int AccountId { get; set; }
        public int FriendId { get; set; }
        public bool Accepted { get; set; } = false;
        public bool AreFriends { get; set; } = false;
        public DateTime BecameFriendsDate { get; set; }
    }
}
