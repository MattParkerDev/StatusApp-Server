using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Application;

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
