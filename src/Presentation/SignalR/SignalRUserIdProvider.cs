using Microsoft.AspNetCore.SignalR;

namespace Presentation.SignalR;

public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User.Identity?.Name;
    }
}
