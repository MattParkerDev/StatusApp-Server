using Microsoft.AspNetCore.SignalR;

namespace StatusApp_Server.Domain;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.Identity?.Name;
    }
}
