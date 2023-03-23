using StatusApp_Server.Domain;

namespace StatusApp_Server.Application.Contracts;

public interface IMessagingService
{
    public List<Message> GetAllMessages(Guid groupId);
}
