using Domain;

namespace Application.Contracts;

public interface IMessagingService
{
    public List<Message> GetAllMessages(Guid chatId);
    List<Guid> GetAllChatIdsByUserName(string userName);
    List<Chat> GetAllChatsByUserName(string userName);
    public Task<Message?> CreateMessageAsUserInGroup(string userName, Guid chatId, string data);
    public Task<Chat?> CreateChatForUsers(List<StatusUser> users);
}
