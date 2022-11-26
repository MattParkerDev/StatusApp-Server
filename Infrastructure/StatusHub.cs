using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Infrastructure
{
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
            var accountId = requestContext.Request.Query["AccountId"];
            var connectionPair = new Connection
            {
                AccountId = Convert.ToInt32(accountId),
                ConnectionId = Context.ConnectionId
            };
            var existingPair = db.Connections.FirstOrDefault(s => s.AccountId == Convert.ToInt32(accountId));
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
            var accountId = Context.GetHttpContext().Request.Query["AccountId"];
            var connectionPair = db.Connections.First(s => s.AccountId == Convert.ToInt32(accountId));
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
    }

    public interface IStatusClient
    {
        // Methods that a client listens for - connection.on(...)
        Task ReceiveMessage(string user, string message);
        Task ReceiveUpdatedUser(User friend);
    }
}