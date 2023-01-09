using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Infrastructure
{
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
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public string GetConnectionUserName()
        {
            return Context.UserIdentifier;
        }
    }

    public interface IStatusClient
    {
        // Methods that a client listens for - connection.on(...)
        Task ReceiveMessage(string user, string message);
        Task ReceiveUpdatedUser(Profile friend);
        Task DeleteFriend(string userName);
    }
}
