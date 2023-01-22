using Microsoft.EntityFrameworkCore;

namespace StatusApp_Server.Domain
{
    [PrimaryKey(nameof(UserName), nameof(FriendUserName))]
    public class Friendship
    {
        public string? UserName { get; set; }
        public string? FriendUserName { get; set; }
        public bool Accepted { get; set; } = false;
        public bool AreFriends { get; set; } = false;
        public DateTime BecameFriendsDate { get; set; }
        public string? FriendFirstName { get; set; }
        public string? FriendLastName { get; set; }
        public Guid GroupId { get; set; }
    }
}
