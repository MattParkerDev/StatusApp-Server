using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Infrastructure;

[Authorize]
public class StatusHub : Hub<IStatusClient>
{
    private readonly IServiceProvider _serviceProvider;

    public StatusHub(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override Task OnConnectedAsync()
    {
        var requestContext = Context.GetHttpContext();
        using var db = requestContext.RequestServices.GetRequiredService<ChatContext>();
        var userName = Context.UserIdentifier;
        var identityUserName = Context.User.Identity.Name;
        if (userName == null || identityUserName == null || userName != identityUserName)
        {
            Context.Abort();
            return base.OnConnectedAsync();
        }

        var connectionPair = new Connection
        {
            UserName = userName,
            ConnectionId = Context.ConnectionId
        };

        db.Connections.Add(connectionPair);
        db.SaveChanges();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        using var scope = _serviceProvider.CreateScope();
        using var db = scope.ServiceProvider.GetRequiredService<ChatContext>();
        var userName = Context.UserIdentifier;
        var identityUserName = Context.User.Identity.Name;
        if (userName == null || identityUserName == null || userName != identityUserName)
        {
            return base.OnDisconnectedAsync(exception);
        }
        var connectionId = Context.ConnectionId;
        var connectionPair = db.Connections.First(
            s => s.ConnectionId == connectionId && s.UserName == userName
        );
        db.Connections.Remove(connectionPair);
        db.SaveChanges();
        return base.OnDisconnectedAsync(exception);
    }

    // Server methods that a client can invoke - connection.invoke(...)
    public async Task BroadcastMessage(string user, string message)
    {
        await Clients.All.ReceiveBroadcast(user, message);
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }

    public string GetConnectionUserName()
    {
        return Context.UserIdentifier;
    }

    public async Task<Message> SendMessage(
        ChatContext db,
        IHubContext<StatusHub, IStatusClient> hubContext,
        Guid groupId,
        string data
    )
    {
        // TODO:Consider checking if are a member of this groupId?
        var userName = Context.UserIdentifier;
        var message = new Message
        {
            GroupId = groupId,
            Data = data,
            AuthorUserName = userName
        };
        db.Messages.Add(message);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return new Message();
        }
        Console.WriteLine(message);
        Console.WriteLine(message.MessageId);
        // TODO:Consider checking if they are friends
        var friendUserName = db.Friendships
            .FirstOrDefault(s => s.UserName == userName && s.GroupId == groupId)
            .FriendUserName;

        var userToNotify = db.Connections
            .Where(s => s.UserName == friendUserName)
            .GroupBy(s => s.UserName)
            .Select(s => s.First().UserName)
            .ToList();

        await hubContext.Clients.Users(userToNotify).ReceiveMessage(message);
        return message;
    }
}

public interface IStatusClient
{
    // Methods that a client listens for - connection.on(...)
    Task ReceiveBroadcast(string user, string message);
    Task ReceiveMessage(Message message);
    Task ReceiveUpdatedUser(Profile friend);
    Task ReceiveUpdatedFriendship(Friendship friendship);
    Task DeleteFriend(string userName);
}
