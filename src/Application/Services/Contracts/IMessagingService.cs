using Domain.Entities;

namespace Application.Services.Contracts;

public interface IMessagingService
{
    public List<Message> GetAllMessages(ChatId chatId);
    List<ChatId> GetAllChatIdsByUserName(string userName);
    List<Chat> GetAllChatsByUserName(string userName);
    public Task<Message?> CreateMessageAsUserInGroup(string userName, ChatId chatId, string data);
    public Task<Chat?> CreateChatForUsers(List<StatusUser> users);
}
