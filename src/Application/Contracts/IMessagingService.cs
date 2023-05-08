using Domain;

namespace Application.Contracts;

public interface IMessagingService
{
    public List<Message> GetAllMessages(Guid groupId);
    public Task<Message?> CreateMessageAsUserInGroup(string userName, Guid groupId, string data);
}
