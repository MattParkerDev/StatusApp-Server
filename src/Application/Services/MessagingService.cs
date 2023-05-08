using Application.Contracts;
using Domain;

namespace Application;

public class MessagingService : IMessagingService
{
    private readonly IStatusContext _db;

    public MessagingService(IStatusContext db)
    {
        _db = db;
    }

    public List<Message> GetAllMessages(Guid groupId)
    {
        return _db.Messages.Where(s => s.GroupId == groupId).ToList();
    }

    public async Task<Message?> CreateMessageAsUserInGroup(
        string userName,
        Guid groupId,
        string data
    )
    {
        var message = new Message
        {
            GroupId = groupId,
            Data = data,
            AuthorUserName = userName
        };
        _db.Messages.Add(message);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return message;
    }
}
