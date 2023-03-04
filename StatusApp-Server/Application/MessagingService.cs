using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class MessagingService : IMessagingService
{
    private readonly StatusContext _db;

    public MessagingService(StatusContext db)
    {
        _db = db;
    }

    public List<Message> GetAllMessages(Guid groupId)
    {
        return _db.Messages.Where(s => s.GroupId == groupId).ToList();
    }
}
