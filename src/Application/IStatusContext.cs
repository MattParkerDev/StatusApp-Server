using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application;

public interface IStatusContext
{
    DbSet<Message> Messages { get; set; }
    DbSet<Friendship> Friendships { get; set; }
    DbSet<SignalRConnection> SignalRConnections { get; set; }
    DbSet<StatusUser> StatusUsers { get; set; }
    DbSet<Chat> Chats { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
