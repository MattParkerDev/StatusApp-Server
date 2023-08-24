using Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;

namespace WebAPI.SignalR;

[Authorize]
public class StatusHub : Hub<IStatusClient>
{
    private readonly StatusContext _db;
    private readonly IMessagingService _messagingService;

    public StatusHub(StatusContext db, IMessagingService messagingService)
    {
        _db = db;
        _messagingService = messagingService;
    }

    public override Task OnConnectedAsync()
    {
        var userName = Context.UserIdentifier;
        var identityUserName = Context.User?.Identity?.Name;
        if (userName == null || identityUserName == null || userName != identityUserName)
        {
            Context.Abort();
            return base.OnConnectedAsync();
        }

        var connectionPair = new SignalRConnection
        {
            UserName = userName,
            ConnectionId = Context.ConnectionId
        };

        _db.SignalRConnections.Add(connectionPair);
        _db.SaveChanges();

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userName = Context.UserIdentifier;
        var identityUserName = Context.User?.Identity?.Name;
        if (userName == null || identityUserName == null || userName != identityUserName)
        {
            return base.OnDisconnectedAsync(exception);
        }
        var connectionId = Context.ConnectionId;
        var connectionPair = _db.SignalRConnections.First(
            s => s.ConnectionId == connectionId && s.UserName == userName
        );
        _db.SignalRConnections.Remove(connectionPair);
        _db.SaveChanges();
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
        return Context.UserIdentifier!;
    }

    public async Task<Message?> SendMessage(
        IHubContext<StatusHub, IStatusClient> hubContext,
        Guid chatId,
        string data
    )
    {
        // TODO:Consider checking if are a member of this groupId?
        var userName = Context.UserIdentifier!;

        var message = await _messagingService.CreateMessageAsUserInGroup(
            userName,
            new ChatId(chatId),
            data
        );

        if (message == null)
        {
            return null;
        }

        // TODO:Consider checking if they are friends
        var friendUserName = _db.Chats
            .Where(s => s.Id.Value == chatId)
            .Select(x => x.ChatParticipants.Select(z => z.UserName).FirstOrDefault())
            .FirstOrDefault();

        var friendConnection = await _db.SignalRConnections.FirstOrDefaultAsync(
            s => s.UserName == friendUserName
        );

        if (friendConnection is not null)
        {
            await hubContext.Clients.Users(friendUserName!).ReceiveMessage(message);
        }
        return message;
    }
}
