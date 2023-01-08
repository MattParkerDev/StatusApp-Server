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
            //var userName = requestContext.Request.Query["userName"].ToString();
            var userName = Context.UserIdentifier;
            if (userName == null)
            {
                Context.Abort();
                return base.OnConnectedAsync();
            }
            var connectionPair = new Connection
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId
            };
            var existingPair = db.Connections.FirstOrDefault(s => s.UserName == userName);
            if (existingPair == null)
            {
                db.Connections.Add(connectionPair);
            }
            else
            {
                existingPair.ConnectionId = Context.ConnectionId;
                db.Connections.Update(existingPair);
            }

            db.SaveChanges();
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            using var scope = _serviceProvider.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<ChatContext>();
            var userName = Context.UserIdentifier;
            if (userName == null)
            {
                return base.OnDisconnectedAsync(exception);
            }
            var connectionPair = db.Connections.First(s => s.UserName == userName);
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
