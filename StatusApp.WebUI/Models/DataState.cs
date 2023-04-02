namespace StatusApp.WebUI.Models;

public class DataState
{
    public StatusUserDto? StatusUser { get; set; }
    public bool Authorized { get; set; }
    public List<StatusUserDto> FriendList { get; set; } = new List<StatusUserDto>();
    public Dictionary<Guid, List<Message>> Messages { get; set; } = new();
    public bool AllFriendsSyncHasRun { get; set; } = false;
}
