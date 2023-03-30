namespace StatusApp.WebUI.Models;

public class DataState
{
    public Profile? UserProfile { get; set; }
    public bool Authorized { get; set; }
    public List<Profile> FriendList { get; set; } = new List<Profile>();
    public Dictionary<Guid, List<Message>> Messages { get; set; } = new();
    public bool AllFriendsSyncHasRun { get; set; } = false;
}
