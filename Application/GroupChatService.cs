using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class GroupChatService
{
    private readonly IServiceProvider _serviceProvider;

    public GroupChatService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task CreateGroupChat(string userName, string friendUserName)
    {
        using var scope = _serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ChatContext>();

        var groupChatEntry1 = new GroupChat { UserName = userName, Admin = true };
        db.GroupChatRegistry.Add(groupChatEntry1);
        var groupChatEntry2 = new GroupChat
        {
            GroupId = groupChatEntry1.GroupId,
            UserName = friendUserName,
            Admin = true
        };
        db.GroupChatRegistry.Add(groupChatEntry2);
        await db.SaveChangesAsync();
    }
}
