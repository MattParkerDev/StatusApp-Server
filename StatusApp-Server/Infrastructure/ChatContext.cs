using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StatusApp_Server.Infrastructure;

public class ChatContext : IdentityUserContext<User>
{
    public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

    // https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
    public virtual DbSet<Message> Messages { get; set; } = null!;
    public virtual DbSet<Friendship> Friendships { get; set; } = null!;
    public virtual DbSet<Connection> Connections { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
