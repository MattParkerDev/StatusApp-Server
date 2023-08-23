namespace WebUI.Models;

public class DataState
{
    public StatusUserDto? StatusUser { get; set; }
    public bool Authorized { get; set; }
    public List<StatusUserDto> Friends { get; set; } = new List<StatusUserDto>();

    //public Dictionary<Guid, List<Message>> Messages { get; set; } = new();
    public List<Chat> Chats { get; set; } = new List<Chat>();
    public List<Friendship> Friendships { get; set; } = new List<Friendship>();
}
