namespace WebUI.Models;

public class DataState
{
    public StatusUserDto? StatusUser { get; set; }
    public bool Authorized { get; set; }
    public List<StatusUserDto> Friends { get; set; } = new List<StatusUserDto>();

    //public Dictionary<Guid, List<Message>> Messages { get; set; } = new();
    public List<ChatDto> Chats { get; set; } = new List<ChatDto>();
    public List<FriendshipDto> Friendships { get; set; } = new List<FriendshipDto>();
}
